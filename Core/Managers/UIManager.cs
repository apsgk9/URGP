using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Service;
using System;
using AddressableFunctions;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.ResourceLocations;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine.EventSystems;
using System.Linq;

public class UIManager : SimpleSingleton<UIManager>, IHasInitialized
{
    private const string UISceneName = "UI";
    private const string PLAYERUIPARENTNAME = "---PLAYERUI---";
    public InputSystemUIInputModule CurrentInputSystemUIInputModule;
    public bool UIHasBeenBuilt { get; private set; }
    public bool hasInitialized {get; private set;}

    public Dictionary<string, GameObject> UIGameObjectList {get; private set;}

    //public UIPlayer UIPlayerReference;
    public bool UserFocusedInMenu;
    private bool _previousUserFocusedInMenu;

    public bool PointerInMenu 
    { get 
        {
            if(EventSystem.current!=null)
            {
                return EventSystem.current.IsPointerOverGameObject();
            }
            return false;
        }
    }

    public Scene UIScene { get; private set; }

    public bool PlayerUILoaded {get {return GetPlayerUIStatus();}}

    

    public GameObject PlayerUIParentGameObject;

    public bool _inputSystemInitialized=false;

    //Lists what menu's the user is currently in. Might not be entirely accurate

    protected override void Awake()
    {
        base.Awake();
        _previousUserFocusedInMenu=UserFocusedInMenu;
    }

    private IEnumerator Start()
    {
        hasInitialized=false;
        Setup();
        while(_inputSystemInitialized==false)
        {
            yield return null;
        }
        hasInitialized=true;
    }
    private void OnEnable()
    {
        GameTransitionManager.Instance.InitiatedSceneTransition += SceneTransitionInitiated;
        
    }

    private void OnDisable()
    {        
        if(GameTransitionManager.CanAccess)
            GameTransitionManager.Instance.InitiatedSceneTransition -= SceneTransitionInitiated;
    }

    private void SceneTransitionInitiated()
    {
        ResetUI();
    }

    private void ResetUI()
    {
        UIHasBeenBuilt=false;
        UIGameObjectList= new Dictionary<string, GameObject>();
    }

    private void Update()
    {
        if(_previousUserFocusedInMenu!=UserFocusedInMenu)
        {
            UpdateActionMap();
            _previousUserFocusedInMenu=UserFocusedInMenu;
        }
        
    }
    private void UpdateActionMap()
    {
        if (UserFocusedInMenu)
        {
            UserInput.Instance.EnableMenuControls();
        }
        else
        {
            UserInput.Instance.EnableGameplayControls();
        }
    }
    public void Setup()
    {
        if(!UIHasBeenBuilt)
        {
            UIGameObjectList= new Dictionary<string, GameObject>();
            SetupInputModule();
            CheckUIScene();
            UIHasBeenBuilt=true;
            UpdateActionMap();
        }
    }

    private void CheckUIScene()
    {
        UIScene=SceneManager.GetSceneByName(UISceneName);
        if(!UIScene.IsValid())
        {
            UIScene = SceneManager.CreateScene(UISceneName);
        }
    }

    

    public IEnumerator LoadPlayerUI()
    {
        //Maybe delete previous PLAYER UI IF IT EXISTS.

        //Debug.Log("LOAD PLAYER UI");
        
        List<string> keys= new List<string>();
        keys.Add(AddressableLabelNames.Player);
        keys.Add(AddressableLabelNames.UI);

        //Task task = AddressableFunctions.Handler.LoadAndAssociateResultWithKey
        //(keys,ProcessUIPlayer,Addressables.MergeMode.Intersection);

        
        Task<Dictionary<string, AsyncOperationHandle<GameObject>>> task = AddressableFunctions.Handler.GetLoadAndAssociateResultWithKey
        (keys,ProcessUIPlayer,Addressables.MergeMode.Intersection);
        while(!task.IsCompleted)
        {
            yield return null;
        }

        var initialOperationDictionary = task.Result;
        Dictionary<string, AsyncOperationHandle<GameObject>> finalSpawnDictionary= new Dictionary<string, AsyncOperationHandle<GameObject>>();

        
        //Check Whether or not the UI Object has already been spawned. If so, remove from spawn list
        foreach (var item in initialOperationDictionary)
        {
            GameObject AssetGameObject = item.Value.Result;
            if(UIGameObjectList.ContainsKey(item.Key))
            {
                continue;
            }
            finalSpawnDictionary.Add(item.Key,item.Value);
            UIGameObjectList.Add(item.Key,item.Value.Result);
        }

        AddressableFunctions.Handler.SpawnOperations(finalSpawnDictionary,ProcessUIPlayer);
    }
    public void ProcessUIPlayer(GameObject obj)
    {
        RenameAndMovetoUIScene(obj);
    }

    private bool GetPlayerUIStatus()
    {
        var UIObjects= UIGameObjectList.Values.ToList();
        if(UIObjects.Count==0)
            return false;

        //Are any Player UI missing
        bool UIQuickMenuFound=false;
        bool UIPauseMenuFound=false;
        bool InteractionMenuHandlerFound=false;
        foreach(var UI in UIObjects)
        {
            if(UIQuickMenuFound==false && UI.GetComponent<UIQuickMenu>()!=null)
            {
                UIQuickMenuFound=true;
            }
            else if(UIPauseMenuFound==false && UI.GetComponent<UIPauseMenu>()!=null)
            {
                UIPauseMenuFound=true;
            }
            else if(InteractionMenuHandlerFound==false && UI.GetComponent<InteractionMenuHandler>()!=null)
            {
                InteractionMenuHandlerFound=true;           
            }

        }        
        return UIQuickMenuFound && UIPauseMenuFound && InteractionMenuHandlerFound;
    }

    public void RenameAndMovetoUIScene(GameObject obj)
    {
        //obj.name = obj.name.Replace("(Clone)", "");
        CheckUIScene();
        SceneManager.MoveGameObjectToScene(obj, SceneManager.GetSceneByName(UISceneName));
    }

    //There really should be only one. might be bad
    private void SetupInputModule()
    {
        var modulesFound = GameObject.FindObjectsOfType<InputSystemUIInputModule>();
        if (modulesFound.Length==0)
        {
            StartCoroutine(LoadEventSystem());
        }        
        else if(modulesFound.Length>0)
        {
            for(int i=1;i<modulesFound.Length;i++)
            {
                Destroy(modulesFound[i].gameObject); 
            }
            StartCoroutine(LoadEventSystem());
        }
    }

    public IEnumerator LoadEventSystem()
    {
        List<string> keys= new List<string>();
        keys.Add(AddressableLabelNames.EventSystem);
        Task task = AddressableFunctions.Handler.LoadAndAssociateResultWithKey
        (keys,ProcessEventSystem,Addressables.MergeMode.Intersection);
        while(!task.IsCompleted)
        {
            yield return null;
        }
        _inputSystemInitialized=true;
    }
    public void ProcessEventSystem(GameObject obj)
    {     
        GameObject.DontDestroyOnLoad(obj);
        CurrentInputSystemUIInputModule = obj.GetComponent<InputSystemUIInputModule>();
    }

}
