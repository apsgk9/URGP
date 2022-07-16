using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using MainObject;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
using UnityEngine;

public class PlayerCameraManager : GameServiceBehaviour<PlayerCameraManager>
{
    public CameraPlayer Current_VirtualPlayerCamera;
    public List<PlayerFreeLookCamera> VirtualFreeLookPlayerCameras;
   

    public PlayerLockOnCamera Current_VirtualLockOnCamera;
    public List<PlayerLockOnCamera> VirtualLockOnCameras;
    public PlayerFreeLookCamera PreviousPlayerFreeLookCamera;

    //This Should Assigned in the inspector
    [SerializeField] private Camera PlayerCamera;
    [SerializeField] private CinemachineBrain _cinemachineBrain;


    private bool _isBlendingStatus;
    public Action StartedBlending;
    public Action FinishedBlending;
    public Action<bool> LockedOnActivate;
    public Action<bool> LockedOnChangeTarget;
    public List<GameObject> PlayerCameraSystems;

    public bool ToggleCameraSystem=true;
    protected override void Awake()
    {
        _isBlendingStatus=_cinemachineBrain.IsBlending;

        UpdateCameras();
        UpdatePlayerCameras();
        base.Awake();
    }
    private void Update()
    {
        foreach (var system in PlayerCameraSystems)
        {
            system.SetActive(ToggleCameraSystem);                
        }

        HandleBlendingStatus();
    }
     
    private void OnEnable()
    {
        GameTransitionManager.Instance.LoadingStatus += LoadingStatusChange;
    }

    private void OnDisable()
    {
        if(GameTransitionManager.CanAccess)
            GameTransitionManager.Instance.LoadingStatus -= LoadingStatusChange;
    }

    private void LoadingStatusChange(bool isLoading)
    {
        foreach (var system in PlayerCameraSystems)
        {
            //Causes problems, better to leave it to individual scripts to disable themeslves.
            //TODO- CHECK LATER 6/14/22
            system.SetActive(!isLoading);                
        }
    }

    
    //public void SetIgnoreTime(bool inputIgnore)
    //{
    //    foreach (var cams in VirtualFreeLookPlayerCameras)
    //    {
    //        cams.SetIgnoreTime(inputIgnore);                
    //    }
//
    //}

    private void HandleBlendingStatus()
    {
        if (_isBlendingStatus != _cinemachineBrain.IsBlending)
        {
            if (_isBlendingStatus == false && _cinemachineBrain.IsBlending)
            {
                StartedBlending?.Invoke();
            }
            else
            {
                FinishedBlending?.Invoke();
            }
            _isBlendingStatus = _cinemachineBrain.IsBlending;
        }
    }

    private void OnValidate()
    {
        _cinemachineBrain=PlayerCamera.GetComponent<CinemachineBrain>();
        UpdateCameras();
    }

    public void UpdateCameras()
    {
#if UNITY_EDITOR
        VirtualFreeLookPlayerCameras = StageUtility.GetCurrentStageHandle().FindComponentsOfType<PlayerFreeLookCamera>().ToList();
#else
        VirtualFreeLookPlayerCameras = GetComponentsInChildren<PlayerFreeLookCamera>().ToList();
#endif

        Current_VirtualPlayerCamera = FindVirtualMainPlayerCamera();
    }

    public bool SetMainPlayerCamera(CameraPlayer pCam)
    {
        if(pCam==null)
            return false;
        if(Current_VirtualPlayerCamera is PlayerFreeLookCamera)
            PreviousPlayerFreeLookCamera=Current_VirtualPlayerCamera as PlayerFreeLookCamera;
            

        Current_VirtualPlayerCamera=pCam;
        pCam.SetAsMain();
        //Debug.Log("SetMainPlayerCamera: "+pCam.name);
        return true;
    }

    public CinemachineBrain GetCinemachineBrain()
    {
        return _cinemachineBrain;
    }

    //Todo: Please recheck this cause it forces a dependency of lockons with associated LockOn cameras rather than a
    //seperate lockon camera
    /*
        1) Gets Associated LockOn From Current VirtualPlayerCamera is if its FreeLook. Returns false if unsusccesful
        2) Sets LockOn Cam Found as being used and sets it as the current virtual camera.
        3) Activates LockOn
    */
    public bool ActivateLockOn(Transform Target)
    {
        if(Current_VirtualLockOnCamera=null)
            return false;        
        
        var LockOnCameraToUse=GetAssociatedLockOnCamera(Current_VirtualPlayerCamera as PlayerFreeLookCamera);//VirtualMainLockOnCamera;
        if(LockOnCameraToUse==null)
            return false;

        if(Current_VirtualPlayerCamera is PlayerFreeLookCamera)
            PreviousPlayerFreeLookCamera=Current_VirtualPlayerCamera as PlayerFreeLookCamera;

        Current_VirtualLockOnCamera=LockOnCameraToUse;
        
        Current_VirtualPlayerCamera=Current_VirtualLockOnCamera;
        
        Current_VirtualLockOnCamera.ActivateLockOn(Target);

        //Debug.Log("ActivateLockOn");
        LockedOnActivate?.Invoke(true);
        LockedOnChangeTarget?.Invoke(Target);
        return true;
    }

    public void ResetCamerasToDefaultStates()
    {
        foreach(var freelook_VM in VirtualFreeLookPlayerCameras)
        {            
            freelook_VM.FreeLookVirtualCam.m_YAxis.Value=0.5f;
            freelook_VM.FreeLookVirtualCam.m_XAxis.Value=GameMode.IPlayerCentric.Player.transform.rotation.eulerAngles.y;
        }

        if(Current_VirtualPlayerCamera is PlayerFreeLookCamera)
        {
            var freeLook=Current_VirtualPlayerCamera as PlayerFreeLookCamera;
            freeLook.FreeLookVirtualCam.UpdateCameraState(Vector3.up,Mathf.Infinity);
        }
    }

    public void ChangeLockOnTarget(Transform Target)
    {
        if(Current_VirtualLockOnCamera!=Current_VirtualPlayerCamera)
        {
            Debug.Log("LockOn Camera is not the main camera being used.");  
            return;
        }

        Current_VirtualLockOnCamera.ChangeTarget(Target);        
        LockedOnChangeTarget?.Invoke(Target);
    }

    private PlayerLockOnCamera GetAssociatedLockOnCamera(PlayerFreeLookCamera freelookCamera)
    {
        foreach(var camLockon in VirtualLockOnCameras)
        {
            if(camLockon.GetAssociatedPlayerFreeLookCamera()== freelookCamera)
                return camLockon;
        }
        return null;                
    }

    public bool DeactivateLockOn()
    {
        if(Current_VirtualPlayerCamera==null || Current_VirtualPlayerCamera!=Current_VirtualLockOnCamera)
            return false;
        Current_VirtualPlayerCamera=PreviousPlayerFreeLookCamera;   
        Current_VirtualLockOnCamera=null; 
        PreviousPlayerFreeLookCamera.SetAsMain();
        
        LockedOnActivate?.Invoke(false);
        return true;
    }

    public CameraPlayer GetNextVacantFreeLookCameraPlayer()
    {
        //Prevent Switching into Associated FreeLookCamera from Lockon
        PlayerFreeLookCamera freeLooktoIgnore=null;
        if(Current_VirtualPlayerCamera is PlayerLockOnCamera)
        {
            var currentLockon= Current_VirtualPlayerCamera as PlayerLockOnCamera;
            freeLooktoIgnore=currentLockon.GetAssociatedPlayerFreeLookCamera();
        }

        foreach(var Vcam in VirtualFreeLookPlayerCameras)
        {
            if(freeLooktoIgnore == Vcam)
                continue;
            if(!Vcam.isMain)
            {
                return Vcam;
            }
        }
        return null;
    }

    private CameraPlayer FindVirtualMainPlayerCamera()
    {
        foreach (var camPlayer in VirtualFreeLookPlayerCameras)
        {
            if(camPlayer.isMain)
            {
                return camPlayer;
            }
        }
        return null;
    }

    private void UpdatePlayerCameras()
    {
        foreach (var camLockon in VirtualLockOnCameras)
        {
            if(camLockon!=Current_VirtualLockOnCamera)
            {
                camLockon.HandleBeingOffstageCamPlayer();           
            }
            else
            {
                camLockon.HandleBeingMainCamPlayer();
            }
        }

        foreach (var camPlayer in VirtualFreeLookPlayerCameras)
        {
            if(camPlayer!=Current_VirtualPlayerCamera)
            {
                camPlayer.HandleBeingOffstageCamPlayer();           
            }
            else
            {
                camPlayer.HandleBeingMainCamPlayer();
            }
        }
    }

    public Camera GetPlayerCamera()
    {
        return PlayerCamera;
    }


    public void AddtionalFocus(Transform newTransformToFocus)
    {
        if(Current_VirtualPlayerCamera is PlayerFreeLookCamera)
        {
            var freeLook= Current_VirtualPlayerCamera as PlayerFreeLookCamera;
            freeLook.AddTargetSoft(newTransformToFocus);
        }
        else if(Current_VirtualPlayerCamera is PlayerLockOnCamera)
        {            
            var lockOn= Current_VirtualPlayerCamera as PlayerLockOnCamera;
            lockOn.GetAssociatedPlayerFreeLookCamera().AddTargetSoft(newTransformToFocus);
        }
        else
        {
            Debug.LogError("Current_VirtualPlayerCamera is of unknown type to add focus.");
        }
    }

    
    public void RemoveAdditionalFocus(Transform transformToRemove)
    {
        if(Current_VirtualPlayerCamera is PlayerFreeLookCamera)
        {
            var freeLook= Current_VirtualPlayerCamera as PlayerFreeLookCamera;
            freeLook.RemoveTargetSoft(transformToRemove);
        }
        
        else if(Current_VirtualPlayerCamera is PlayerLockOnCamera)
        {            
            var lockOn= Current_VirtualPlayerCamera as PlayerLockOnCamera;
            lockOn.GetAssociatedPlayerFreeLookCamera().RemoveTargetSoft(transformToRemove);
        }
        else
        {
            Debug.LogError("Current_VirtualPlayerCamera is of unknown type to remove focus.");
        }
    }
}
