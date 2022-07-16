using System;
using System.Collections;
using System.Collections.Generic;
using GameStatics;
using Service;
using UnityEngine;

[Serializable]
public class GameInitialization
{
    [SerializeField]
    private bool shouldInitialize = true;
    [SerializeField]
    public UIManager UIManagerReference;
    [SerializeField]
    public UserInput UserInputSystemReference;
    [SerializeField]
    public SettingsManager SettingsManagerReference;
    [SerializeField]
    public AudioManager AudioManagerReference;
    [SerializeField]
    public CameraManager CameraManagerReference;

    [SerializeField]
    public GameTransitionManager TransitionManagerReference;
    private GameObject _managerParent;

    [SerializeField]
    private bool _hasInitialized = false;
    private bool _UIManagerInitialized;
    private bool _inputSystemInitialized;
    private bool _userSettingsInitialized;
    private bool _AudioManagerInitialized;
    private bool _cameraManagerInitialized;
    private bool _transitionManagerInitialized;

    //initialize checks
    public bool HasInitialized { get { return _hasInitialized; } }

    public GameInitialization()
    {
        _hasInitialized=false;
    }

    public void InitializeSystems()
    {
        if(StaticFunctions.FindObjectOfType<DontInitialize>())
            return;
        if (!shouldInitialize)
            return;
        StaticFunctions.Instance.StartCoroutine((Initialize()));
    }
    private IEnumerator Initialize()
    {
        if (!shouldInitialize)
            yield return null;
        ResetInitializationToFalse();

        if (ServiceLocator.Current == null)
            ServiceLocator.Initialize();

        CreateManagerParent();

        Create<SettingsManager>();
        while (_userSettingsInitialized == false || !SettingsManagerReference.hasInitialized)
        {
            yield return null;
        }


        Create<AudioManager>();
        while (_AudioManagerInitialized == false || !AudioManagerReference.hasInitialized)
        {
            yield return null;
        }

        Create<GameTransitionManager>();

        while (_transitionManagerInitialized == false || !TransitionManagerReference.hasInitialized)
        {
            yield return null;
        }
        
        Create<UIManager>();

        while (_UIManagerInitialized == false)
        {
            yield return null;
        }        

        Create<UserInput>();

        while (_inputSystemInitialized == false )
        {
            yield return null;
        }

        
        Create<CameraManager>();

        while (_cameraManagerInitialized == false )
        {
            yield return null;
        }


        
    }

    public void ResetInitializationToFalse()
    {
        _hasInitialized = false;
        _UIManagerInitialized = false;
        _inputSystemInitialized = false;
        _userSettingsInitialized = false;
        _AudioManagerInitialized = false;
        _cameraManagerInitialized = false;
        _transitionManagerInitialized=false;
    }
    public void WrapUpObject<T>(T Component) where T : MonoBehaviour
    {
        UpdateInitializationCheck(Component);
        ParentToManagers(Component.gameObject);
        CheckifHasBeenInitialized(Component);
        Rename(Component);
    }

    private void CheckifHasBeenInitialized<T>(T componentReference) where T : MonoBehaviour
    {
        if (_userSettingsInitialized && _inputSystemInitialized && _userSettingsInitialized)
        {
            _hasInitialized = true;
        }
    }
    private void Create<T>() where T : MonoBehaviour
    {
        var monoFound = GameObject.FindObjectOfType<T>();
        if (monoFound == null)
        {
            var reference=InstantLoad<T>();
            WrapUpObject<T>(reference as T);
        }
        else
        {
            
            SetReference<T>(ref monoFound);
            GameObject.DontDestroyOnLoad(monoFound.gameObject);
            WrapUpObject<T>(monoFound.GetComponent<T>());
        }
    }

    private void CreateManagerParent()
    {
        if (!GameObject.Find("--MANAGERS--"))
        {
            _managerParent = new GameObject("--MANAGERS--");
            GameObject.DontDestroyOnLoad(_managerParent);
        }
    }

    private void SetReference<T>(ref T script) where T : MonoBehaviour
    {
        if (typeof(T).Name == typeof(UserInput).Name)
        {
            UserInputSystemReference=script as UserInput;
        }
        else if (typeof(T).Name == typeof(UIManager).Name)
        {
            UIManagerReference=script as UIManager;
        }
        else if (typeof(T).Name == typeof(SettingsManager).Name)
        {
            SettingsManagerReference=script as SettingsManager;
        }
        else if (typeof(T).Name == typeof(AudioManager).Name)
        {
            AudioManagerReference=script as AudioManager;
        }
        else if (typeof(T).Name == typeof(CameraManager).Name)
        {
            CameraManagerReference=script as CameraManager;
        }
        else if (typeof(T).Name == typeof(GameTransitionManager).Name)
        {
            TransitionManagerReference=script as GameTransitionManager;
        }
        else
        {
            Debug.LogError("Cannot Set Reference.");
        }
        
    }    

    private MonoBehaviour GetReference<T>() where T : MonoBehaviour
    {
        if (typeof(T).Name == typeof(UserInput).Name)
        {
            return UserInputSystemReference;
        }
        else if (typeof(T).Name == typeof(UIManager).Name)
        {
            return UIManagerReference;
        }
        else if (typeof(T).Name == typeof(SettingsManager).Name)
        {
            return SettingsManagerReference;
        }
        else if (typeof(T).Name == typeof(AudioManager).Name)
        {
            return AudioManagerReference;
        }
        else if (typeof(T).Name == typeof(CameraManager).Name)
        {
            return CameraManagerReference;
        }
        else if (typeof(T).Name == typeof(GameTransitionManager).Name)
        {
            return TransitionManagerReference;
        }
        return null;
    }

    //Loads an instance of the associated script and puts it on a gameobject
    private T InstantLoad<T>() where T : MonoBehaviour
    {
        var newObj=new GameObject();
        var monoscript=newObj.AddComponent<T>();
        SetReference<T>(ref monoscript);
        //Debug.Log("DEBUG INSTANTLOAD: "+typeof(T).Name);

        Type type = typeof(T);
        if (monoscript == null)
        {
            Debug.LogError($"{typeof(T).Name} component not found.");
            return null;
        }
        GameObject.DontDestroyOnLoad(monoscript.gameObject);
        return monoscript;
    }

    private void ParentToManagers(GameObject gameObject)
    {
        gameObject.transform.SetParent(_managerParent.transform);
    }

    private void Rename<T>(T component) where T : MonoBehaviour
    {
        component.name =  typeof(T).FullName.ToUpper();
    }

    private void UpdateInitializationCheck<T>(T componentReference) where T : MonoBehaviour
    {
        if (componentReference is SettingsManager)
            _userSettingsInitialized = true;
        if (componentReference is UserInput)
            _inputSystemInitialized = true;
        if (componentReference is UIManager)
            _UIManagerInitialized = true;
        if (componentReference is AudioManager)
            _AudioManagerInitialized = true;
        if (componentReference is CameraManager)
            _cameraManagerInitialized = true;
        if (componentReference is GameTransitionManager)
            _transitionManagerInitialized = true;
    }
}