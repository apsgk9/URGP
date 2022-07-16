using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class GameTransitionManager : Singleton<GameTransitionManager> , IHasInitialized
{
    public ISceneTransitionHandler TransitionHandler;
    public bool isFading=false;
    public bool isTransitioning {get; private set;}
    public Action InitiatedSceneTransition;
    public Action<bool> LoadingStatus;

    public enum FadeType
    {
        In=0, Out=1
    }

    private SystemSettings _systemSettings;

    public bool hasInitialized {get;private set;}
    public const string LOADINGSCREESCENENAME = "Scene_LoadingScreen";

    public IEnumerator Start() 
    {
        isTransitioning=false;
        hasInitialized=false;
        List<string> keys= new List<string>();
        keys.Add(AddressableLabelNames.TransitionHandler);
        Task task = AddressableFunctions.Handler.LoadAndAssociateResultWithKey
        (keys,ProcessTransitionHandler,Addressables.MergeMode.Intersection);
        while(!task.IsCompleted)
        {
            yield return null;
        }
        TransitionHandler.SetPercent(1);        
        _systemSettings= Service.ServiceLocator.Current.Get<SettingsManager>().GetSystemSettings();
        hasInitialized=true;
    }

    private void ProcessTransitionHandler(GameObject obj)
    {
        if(obj.GetComponent<ISceneTransitionHandler>()!=null)
        {
            TransitionHandler=obj.GetComponent<ISceneTransitionHandler>();
            DontDestroyOnLoad(obj);
        }
    }

    public void StandardFadeIn()
    {
        Fade(_systemSettings.DefaultInitialWaitTimeBeforeTransitionDuration,_systemSettings.DefaultSceneFadeTransitionDuration,GameTransitionManager.FadeType.In);
    }

    public float StandardFadeOut()
    {
        Fade(_systemSettings.DefaultInitialWaitTimeBeforeTransitionDuration,_systemSettings.DefaultSceneFadeTransitionDuration,FadeType.Out);

        return _systemSettings.DefaultInitialWaitTimeBeforeTransitionDuration+_systemSettings.DefaultSceneFadeTransitionDuration;

    }

    public void Fade(float initialWaitTime, float time, FadeType type)
    {

        if(isFading)
            return;
        
        if(TransitionHandler==null)
        {
            Debug.Log("No TransitionHandler exists");
            return;
        }
        StartCoroutine(StartFadeWithWaitTime(initialWaitTime,time,type));
    }

    public void Fade(float time, FadeType type)
    {

        if(isFading)
            return;
        
        if(TransitionHandler==null)
        {
            Debug.Log("No TransitionHandler exists");
            return;
        }


        StartCoroutine(StartFade(time,type));

    }

    // Use Unscaled Time since Scene Transitions are not gameplay functions so they aren't tied to game related actions.
    private IEnumerator StartFadeWithWaitTime(float initialWaitTime, float duration, FadeType type)
    {
        float endWaitTime=Time.unscaledTime+initialWaitTime;
        isFading=true;
        while(endWaitTime >Time.unscaledTime)
        {
            yield return null;            
        }
        StartCoroutine(StartFade(duration,type));
    }

    private IEnumerator StartFade(float duration, FadeType type)
    {
        if(TransitionHandler!=null)
        {
            isFading=true;
            float CurrentPercent;
            float endTime=Time.unscaledTime+duration;

            
            while(endTime >Time.unscaledTime)
            {

                CurrentPercent= (endTime-Time.unscaledTime)/duration;

                if(type==FadeType.Out)
                {
                    CurrentPercent=1-CurrentPercent;
                }

                TransitionHandler.SetPercent(CurrentPercent);

                yield return null;
            }
        }
        else
        {
            Debug.Log("No TransitionHandler exists");
            yield return null;
        }
        
        TransitionHandler.SetPercent((int)type);
        isFading=false;
    }




    public void LoadScene(AssetReference SceneToLoad)
    {
        if(isTransitioning)
            return;
        //This has to be called in here cause the 
        StartCoroutine(TransitionToNewScene(SceneToLoad));
    }

    private IEnumerator TransitionToNewScene(AssetReference SceneToLoad)
    {
        isTransitioning=true;
        float FadeOutDuration = GameTransitionManager.Instance.StandardFadeOut();
        
        InitiatedSceneTransition?.Invoke();
        while (GameTransitionManager.Instance.isFading)
            yield return null;
        
        //Load IntoLoading Screen

        LoadingStatus?.Invoke(true);
        AsyncOperationHandle<SceneInstance> LoadingHandle = Addressables.LoadSceneAsync(LOADINGSCREESCENENAME, LoadSceneMode.Single);
        yield return LoadingHandle;

        yield return null;

        AsyncOperationHandle<SceneInstance> sceneHandle = Addressables.LoadSceneAsync(SceneToLoad, LoadSceneMode.Single);
        yield return sceneHandle;
        

        sceneHandle.Completed += SceneCompleted;
    }

    private void SceneCompleted(AsyncOperationHandle<SceneInstance> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded) 
        { // Set our reference to the AsyncOperationHandle (see next section) 
            Debug.Log(obj.Result.Scene.name + " successfully loaded."); 
            var loadedScene = obj.Result;
            // (optional) do more stuff
            
            LoadingStatus?.Invoke(false);
            isTransitioning=false;
        }
        else
        {
            Debug.LogError("LOADING FAILED.");
        }
    }

}
