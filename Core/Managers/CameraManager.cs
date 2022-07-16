using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cinemachine;
using MainObject;
using Service;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public class CameraManager : Singleton<CameraManager>, IHasInitialized
{
    public bool hasInitialized {get;private set;}
    public GameObject CameraParent;
    public string CameraParentName = "CAMERA_SYSTEM_HOLDER";

    private bool _CameraManagerHasBeenBuilt;
    public PlayerCameraManager PlayerCameraManagerReference;
    public const string CameraSceneName ="Cameras";

    private IEnumerator Start()
    {
        hasInitialized=false;
        Setup();
        while(_CameraManagerHasBeenBuilt==false)
        {
            yield return null;
        }
        hasInitialized=true;
    }


    public Camera GetPlayerCamera()
    {
        if(PlayerCameraManagerReference)
        {
            return PlayerCameraManagerReference.GetPlayerCamera();
        }
        return null;
    }
    public Transform GetCurrentVirtualCamTransform()
    {
        if(PlayerCameraManagerReference)
        {
            return PlayerCameraManagerReference.Current_VirtualPlayerCamera.transform;
        }
        return null;
    }
    public bool SetMainCameraPlayer(CameraPlayer pCam)
    {
        if(PlayerCameraManagerReference)
        {
            return PlayerCameraManagerReference.SetMainPlayerCamera(pCam);
        }
        return false;
    }

    public CameraPlayer GetNextFreelookPlayerCameraToChangedTo()
    {
        if(PlayerCameraManagerReference)
        {
            return PlayerCameraManagerReference.GetNextVacantFreeLookCameraPlayer();
        }
        return null;
    }

    public CinemachineBrain GetPlayerCameraCinemachineBrain()
    {
        if(PlayerCameraManagerReference==null)
            return null;

        return PlayerCameraManagerReference.GetCinemachineBrain();
    }



#region Initilization
    private void Setup()
    {        
        SetupCameraParent();
        StartCoroutine(LoadCameras());
        
    }

    private void SetupCameraParent()
    {
        if (CameraParent == null)
        {
            if (GameObject.Find(CameraParentName) == null)
            {
                CameraParent = new GameObject();
                CameraParent.name = CameraParentName;
            }
        }
        DontDestroyOnLoad(CameraParent);
    }

    //private void SetupUIScene()
    //{
    //    var UIScene=SceneManager.GetSceneByName(CameraSceneName);
    //    if(!UIScene.IsValid())
    //    {
    //        SceneManager.CreateScene(CameraSceneName);
    //    }
    //}

    public IEnumerator LoadCameras()
    {
        _CameraManagerHasBeenBuilt=false;
        List<string> keys= new List<string>();
        keys.Add(AddressableLabelNames.CameraManager);
        Task task = AddressableFunctions.Handler.LoadAndAssociateResultWithKey
        (keys,ProcessCameras,Addressables.MergeMode.Union);
        while(!task.IsCompleted)
        {
            yield return null;
        }
        
        _CameraManagerHasBeenBuilt=true;
    }

    

    public void ProcessCameras(GameObject obj)
    {
        if(obj.GetComponent<PlayerCameraManager>())
        {
            PlayerCameraManagerReference=obj.GetComponent<PlayerCameraManager>();
        }
        UtilityFunctions.GameObjectUtil.ParentToContainer(ref CameraParent,CameraParentName,obj.transform);
        //RenameAndMovetoUIScene(obj);
    }

    //public static void RenameAndMovetoUIScene(GameObject obj)
    //{
    //    //obj.name = obj.name.Replace("(Clone)", "");
    //    SceneManager.MoveGameObjectToScene(obj, SceneManager.GetSceneByName(CameraSceneName));
    //}

    
    #endregion
}