using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using MainObject;
using UnityEngine;

public class PlayerLockOnCamera : CameraPlayer
{
    [Header("PlayerLockOnCamera")]
    public CinemachineTargetGroup CinemachineTargetGroup;
    public CinemachineVirtualCamera LockOnVirtualCamera;
    public static PlayerLockOnCamera CurrentPlayerLockOnCamera;

    [SerializeField] [ReadOnly]
    private PlayerLockOnCamera CurrentPlayerLockOnCameraRef;
    [Header("FreeLook")]
    [SerializeField] [Tooltip("Will copy FreeLook transform when not locked, and  FreeLook copy LockOn transform when locked.")]
    private PlayerFreeLookCamera AssociatedPlayerFreeLookCamera;
    private bool isCameraBlending;
    private bool _hasRegistered;

    protected override void Awake()
    {
        base.Awake();
        if(LockOnVirtualCamera)
        {
            LockOnVirtualCamera.enabled=false;
        }
        CinemachineTargetGroup.m_Targets = new CinemachineTargetGroup.Target[0];
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        //MainObject<CameraPlayer>.OldMainObjectHasBeenReplaced -= PreviousMainCamera;
    }

     
    protected override void OnEnable()
    {
        base.OnEnable();
        Register();
    }

    

    protected override void OnDisable()
    {
        base.OnDisable();
        Deregister();
    }
    private void Register()
    {
        if(_hasRegistered)
            return;
        if(CameraManager.Instance.PlayerCameraManagerReference==null)
            return;
        isCameraBlending=CameraManager.Instance.PlayerCameraManagerReference.GetCinemachineBrain().IsBlending;
        CameraManager.Instance.PlayerCameraManagerReference.StartedBlending += CameraStartedBlending;
        CameraManager.Instance.PlayerCameraManagerReference.FinishedBlending += CameraFinishedBlending;
        _hasRegistered=true;
    }

    private void Deregister()
    {
        if(CameraManager.Instance==null || CameraManager.Instance.PlayerCameraManagerReference==null)
            return;
        CameraManager.Instance.PlayerCameraManagerReference.StartedBlending -= CameraStartedBlending;
        CameraManager.Instance.PlayerCameraManagerReference.FinishedBlending -= CameraFinishedBlending;
        _hasRegistered=false;
    }

    private void CameraFinishedBlending()
    {
        isCameraBlending=false;
    }

    private void CameraStartedBlending()
    {
        isCameraBlending=true;
    }

    protected override void Update()
    {
        base.Update();
        Register();
        HandleAssociatedFreeLookCamera();
    }
    private void LateUpdate()
    {    
    }

    private void HandleAssociatedFreeLookCamera()
    {
        if (AssociatedPlayerFreeLookCamera == null)
            return;
        if (AssociatedPlayerFreeLookCamera.FreeLookVirtualCam == null)
            return;
            
        if (LockOnVirtualCamera == null)
            return;

        //This Pair isn't used
        if (!AssociatedPlayerFreeLookCamera.isMain && !isMain)
            return;

        if (IsLockedOn())
        {
            
            if(LockOnVirtualCamera.enabled==false)
            {
                LockOnVirtualCamera.enabled=true;
            }
            if(!isCameraBlending)
            {
                MatchFreeLookCamToLockOnCam();
            }
        }
        else
        {
            if(LockOnVirtualCamera.enabled==true)
            {
                LockOnVirtualCamera.enabled=false;
            }
        }
    }

    public void ChangeTarget(Transform target)
    {
        SetLookAt(target);
    }

    private void MatchFreeLookCamToLockOnCam()
    {
        AssociatedPlayerFreeLookCamera.FreeLookVirtualCam.ForceCameraPosition(LockOnVirtualCamera.transform.localPosition, LockOnVirtualCamera.transform.localRotation);
    }

    internal void ActivateLockOn(Transform target)
    {
        SetLookAt(target);
        SetAsMain();
        isCameraBlending=true;
    }
    public void SetLookAt(Transform target)
    {
        LockOnVirtualCamera.LookAt=target;
    }

    private bool IsLockedOn()
    {
        return isMain;
    }
    

    //LockOn Camera always needs to follow the current player except when it has an associated
    // freelook camera
    protected override void RequestToSwitchCameraToNewPlayer(GameObject previousPlayer,GameObject changedPlayerObject)
    {
        //Debug.Log("REQUEST SWITCH TO NEW PLAYER: "+ name);
        //Debug.Log("PLAYER HAS CHANGED: "+gameObject.name);
        if(!isMain && !AssociatedPlayerFreeLookCamera.isMain)
        {
            //Debug.Log("DENIED: "+ name);
            return;
        }
        if(AssociatedPlayerFreeLookCamera==null)
        {
            //Debug.Log("GRANTED NULL: "+ name);
            SetMainFocusTo(changedPlayerObject.transform);
            return;
        }
        // Paired with a FreeLookCamera. This is so that both FreeLookCamera and LockOn Camera
        // Follow the same object.
        //Debug.Log("GRANTED: "+ name);
        SetMainFocusTo(AssociatedPlayerFreeLookCamera.FreeLookVirtualCam.Follow);        
    }

    public override void SetMainFocusTo(Transform target)
    {
        base.HandleNewMainFocusedTarget(target);
        AssignToFreeLookCamera();
    }

    

    private void AssignToFreeLookCamera()
    {
        CinemachineTargetGroup.RemoveMember(MainTransformToFocus);
        //Debug.Log("NEW MAIN FOCUS: "+ name + " | "+MainTransformToFocus);
        CinemachineTargetGroup.AddMember(MainTransformToFocus, 1f, 1f);
        LockOnVirtualCamera.enabled = true;
    }

    //Seems like its still going even if I turn it off
    public override void  HandleBeingMainCamPlayer()
    {
        //Debug.Log(gameObject.name+ " is now main camera.");
        base.HandleBeingMainCamPlayer();
        LockOnVirtualCamera.Priority=Priority+1;
        LockOnVirtualCamera.enabled=true;
    }
    
    public override void  HandleBeingOffstageCamPlayer()
    {
        //Debug.Log(gameObject.name+ " is now offstage camera.");
        base.HandleBeingOffstageCamPlayer();
        LockOnVirtualCamera.Priority=Priority;
        LockOnVirtualCamera.enabled=false;
    }    

    protected override void OnValidate()
    {
        
        if(LockOnVirtualCamera)
        {
            LockOnVirtualCamera.Priority=Priority;
        }
        base.OnValidate();        
        
    }

    public PlayerFreeLookCamera GetAssociatedPlayerFreeLookCamera()
    {
        return AssociatedPlayerFreeLookCamera;
    }
}
