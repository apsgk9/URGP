using System.Collections;
using System.Collections.Generic;
using Interaction;
using UnityEngine;
using UnityEngine.InputSystem;
using Attachment;
using UnityEngine.AddressableAssets;
using Follow;
using System;
/*
    Still kinda works even though recentering happens at the beginning of the frame.
    Bit easier than forcing script execution order. (Though apparently that kinda doesn't work)
*/
public class InteractorScreenSpaceLockOn : ScreenInteractor<ILockables>
{
    [Header("Lock On")]
    public Transform CurrentLockOnTarget;
    public PlayerCameraManager PlayerCameraManager;
    public List<RecenterToPlayerForward> RecenterCameraScripts;
    public AssetReferenceGameObject LockOnUI;

    public Vector3 OriginOffset;

    private FollowHandler FollowHandlerRef;
    private GameObject FollowHandlerGO;

    [ReadOnly]
    public bool lockedOn=false;
    public float LockOnChangeCooldown=0.35f;
    private float _lockOnChangeStart;

#if UNITY_EDITOR
    [ReadOnly]
    public bool _debugLockonBool;
#endif

    [Min(0)]
    public float CursorThreshold =5f;
    [Range(0,1)]
    public float StickThreshold =0.25f;
    private bool _SceneTransitioning;
    public bool _cameraChangingToNewPlayer;

    void Awake()
    {
        Setup();
        SpawnLockOn();
    }

    private void Setup()
    {
        _lockOnChangeStart+=Time.time+LockOnChangeCooldown;
    }

    private void SpawnLockOn()
    {
        if (LockOnUI.RuntimeKeyIsValid() == false)
        {
            Debug.LogError("Invalid Key " + LockOnUI.RuntimeKey.ToString());
            Debug.LogError("Aborting Spawning LockOn");
            return;
        }

        LockOnUI.InstantiateAsync().Completed += (asyncOperationHandle) =>
        {
            GameObject resultGO = asyncOperationHandle.Result;
            AddressableFunctions.Handler.AddNotifyOnDestroy(LockOnUI, resultGO);

            FollowHandlerRef= Follow.FollowUtil.AddFollowScript(Vector3.zero, null, resultGO,true);
            FollowHandlerGO=FollowHandlerRef.gameObject;
            FollowHandlerGO.SetActive(false);
        };
    }

    virtual protected void OnEnable()
    {
        UserInput.Instance.PlayerInputActions.PlayerControls.LockOn.performed += LockOnPressed;
        GameMode.IPlayerCentric.PlayerHasChangedTo += PlayerHasChanged;

        //PlayerCameraManager.FinishedBlending += PlayerCameraFinishedBlending;
        
        if(GameTransitionManager.CanAccess && _SceneTransitioning==false)
        {
            GameTransitionManager.Instance.InitiatedSceneTransition += SceneTransitioned;
            GameTransitionManager.Instance.LoadingStatus += LoadingStatusChange;
        }    
    }

   

    virtual protected void OnDisable()
    {
        if(UserInput.CanAccess)
            UserInput.Instance.PlayerInputActions.PlayerControls.LockOn.performed -= LockOnPressed;
        GameMode.IPlayerCentric.PlayerHasChangedTo -= PlayerHasChanged;
        
        //PlayerCameraManager.FinishedBlending -= PlayerCameraFinishedBlending;
        

        if(GameTransitionManager.CanAccess && _SceneTransitioning==false)
        {
            GameTransitionManager.Instance.InitiatedSceneTransition -= SceneTransitioned;
            GameTransitionManager.Instance.LoadingStatus -= LoadingStatusChange;
        }
    }

    private void LoadingStatusChange(bool isLoading)
    {
        if(isLoading==false) //FinishedLoading
        {
            if(FollowHandlerGO==null)
                SpawnLockOn();
            _SceneTransitioning=false;
        }
    }

    private void SceneTransitioned()
    {
        ResetLockOn();
        _SceneTransitioning=true;
    }

    //------TODO CHANGE FROM BASE OF CHARACTER TO LINE OF SIGHT FROM EYES------
    private void PlayerHasChanged(GameObject previousPlayer,GameObject newPlayerObj)
    {
        ResetLockOn();
        Origin=newPlayerObj.transform;
        //if(previousPlayer!=null && previousPlayer!=newPlayerObj)
        //    _cameraChangingToNewPlayer = true;
    }
    //ASSUMES THAT EVERYTIME A NEW PLAYER IS CHANGED TO, THE CAMERA NEEDS TO BLEND
   // private void PlayerCameraFinishedBlending()
   //{
   //    if(_cameraChangingToNewPlayer)
   //    {
   //        _cameraChangingToNewPlayer=false;
   //    }
   //}

    

    protected override void Update()
    {
        if(GameState.isPaused || _SceneTransitioning)
            return;
        
        //Did it this way so it only updates automatically if player is lockedon. Otherwise, should
        //update when only pressed by lockedon button.

#if UNITY_EDITOR
        _debugLockonBool=IsOnLockOnChangeCooldown();
#endif
        if(lockedOn==false) 
            return;

        //Prevents locking on from target behind you and it going off screen.
        //Thereby there are no interatables on screen.
        if( PlayerCameraManager.GetCinemachineBrain().IsBlending)
        {
            return;
        }
        
        //Check to see if they are still within range, if not unlock;
        UpdateInteractables();
        lockedOn=InteractablesFound;
        if(lockedOn==false)
        {
            ResetLockOn();
            return;
        }

        if(CurrentLockOnTarget==null)
        {
            Debug.LogWarning("CurrenLockOnTarget is null");
            return;
        }

        HandleTargetChange();

    }

    private void HandleTargetChange()
    {
        if(ScreenInteractables.Count<=1)
            return;
        if (IsOnLockOnChangeCooldown())
            return;
        //Get Aims
        Vector2 RawCursorDeltaPosition = UserInput.Instance.MouseCursorDeltaPosition;
        Vector2 RawControllerAim = UserInput.Instance.RawControllerAim;

        //Check Thresholds
        float RawCursorDeltaPositionSqrMag = Vector2.SqrMagnitude(RawCursorDeltaPosition);
        bool mouseThreshold = RawCursorDeltaPositionSqrMag < (this.CursorThreshold * this.CursorThreshold);
        bool controllerThreshold =  RawControllerAim.sqrMagnitude < (this.StickThreshold * this.StickThreshold);

        if (mouseThreshold && controllerThreshold)
            return;

        //Debug.Log("RawCursorDeltaPosition: "+RawCursorDeltaPosition);
        //Debug.Log("RawControllerAimNormalized: "+RawControllerAim.normalized);

        Vector2 ChangeDirection;

        //Figure Out Which One to Use, Contropller or Mouse
        if(RawControllerAim.sqrMagnitude < RawCursorDeltaPositionSqrMag)
            ChangeDirection = RawCursorDeltaPosition.normalized;
        else
            ChangeDirection = RawControllerAim.normalized;

        //Debug.Log("ChangeDirection: "+ChangeDirection);
        //Get Current Screen Position of the current Target, so that we find which target to get from this position
        Vector2 CurrentScreenPos=GetNormalizedScreenPosOfTargetFromPlayerCam(CurrentLockOnTarget.position);
        //Debug.Log("ChangeDirection: "+ChangeDirection);

        CurrentLockOnTarget = GetClosestLockOnTargetFromScreenPosition(CurrentScreenPos,ChangeDirection,CurrentLockOnTarget);       

        PlayerCameraManager.ChangeLockOnTarget(CurrentLockOnTarget);
        
        if(FollowHandlerGO!=null)
        {
            FollowHandlerRef.ToFollow=CurrentLockOnTarget;
            if(FollowHandlerGO.activeSelf==false)
                FollowHandlerGO.SetActive(true);
        }
        _lockOnChangeStart=Time.time;
    }

    private bool IsOnLockOnChangeCooldown()
    {
        return Time.time < _lockOnChangeStart + LockOnChangeCooldown;
    }

    private void LockOnPressed(InputAction.CallbackContext obj)
    {
        if(GameState.isPaused)
            return;
        if(PlayerCameraManager== null)
        {
            Debug.LogWarning("Cannot lock on. No PlayerCameraManager available.");
            return;
        }

        //if(_cameraChangingToNewPlayer)
        //{
        //    return;
        //}
       
            
        //Debug.Log("LOCKEDON");
        if(lockedOn)
        {
            //Debug.Log("RESET");
            ResetLockOn();
            
        }
        else
        {
            //Debug.Log("ATTEMPT TO LOCKON");
            UpdateInteractables();
            bool success=AssignCenterLockOnTarget();
            lockedOn=success;
            //Debug.Log("SUCESS: "+success);
        }
    }

    private bool AssignCenterLockOnTarget()
    {
        if(InteractablesFound)
        {
            DisallowRecenter();

            CurrentLockOnTarget = GetCenterLockOnTarget();
            PlayerCameraManager.ActivateLockOn(CurrentLockOnTarget);

            if (FollowHandlerGO != null)
            {
                FollowHandlerRef.ToFollow = CurrentLockOnTarget;
                if (FollowHandlerGO.activeSelf == false)
                    FollowHandlerGO.SetActive(true);
            }

            return true;
        }
        return false;
    }

    

    private Transform GetClosestLockOnTargetFromScreenPosition(Vector2 point,Vector2 Direction, Transform originalTransform)
    {
        if(ScreenInteractables.Count==0)
            return null;
        
        const double ANGLE_TOLERANCE = 0.7;
        Transform ClosestFromTheCenter=originalTransform;
        Vector2 ClosestScreenPos= point;
        
        foreach(var screenInteractable in ScreenInteractables)
        {
            if(ClosestFromTheCenter==null)
            {
                NewClosestInteractable(out ClosestFromTheCenter, out ClosestScreenPos, screenInteractable);
                continue;
            }
            //1)    Check Angle            

            Vector2 lhs = (ClosestScreenPos - point).normalized;
            Vector2 rhs = (screenInteractable.ScreenPosition-point).normalized;
            
            float oldAngle= Vector2.Dot(lhs, Direction);
            float newAngle= Vector2.Dot(rhs,Direction);

            if(newAngle>0 && oldAngle<0)
            {
                NewClosestInteractable(out ClosestFromTheCenter, out ClosestScreenPos, screenInteractable);
                continue;
            }
            if(newAngle>=ANGLE_TOLERANCE && oldAngle<ANGLE_TOLERANCE)
            {
                NewClosestInteractable(out ClosestFromTheCenter, out ClosestScreenPos, screenInteractable);
                continue;
            }

            if (oldAngle >= ANGLE_TOLERANCE && newAngle<ANGLE_TOLERANCE)
            {
                continue;
            }

            float oldDist=Vector2.Distance(ClosestScreenPos,point)/oldAngle;
            float newDistance=Vector2.Distance(screenInteractable.ScreenPosition,point)/newAngle;

            //2)    Check Distance
            if(newDistance< oldDist)
            {
                NewClosestInteractable(out ClosestFromTheCenter, out ClosestScreenPos, screenInteractable);                
            }
            

        }

        return ClosestFromTheCenter;

        static void NewClosestInteractable(out Transform ClosestFromTheCenter, out Vector2 ClosestScreenPos, ScreenInteractable<ILockables> screenInteractable)
        {
            ClosestFromTheCenter = screenInteractable.Interactable.gameObject.transform;
            ClosestScreenPos = screenInteractable.ScreenPosition;
        }
    }
    private Transform GetCenterLockOnTarget()
    {
        if(ScreenInteractables.Count==0)
            return null;
        
        Transform ClosestFromTheCenter=ScreenInteractables[0].Interactable.gameObject.transform;
        Vector2 ClosestScreenPost=ScreenInteractables[0].ScreenPosition;

        for(int i=1;i<ScreenInteractables.Count;i++)
        {
            if(Vector2.SqrMagnitude(ClosestScreenPost)> Vector2.SqrMagnitude(ScreenInteractables[i].ScreenPosition))
            {
                ClosestScreenPost=ScreenInteractables[i].ScreenPosition;
                ClosestFromTheCenter = ScreenInteractables[i].Interactable.gameObject.transform;
            }
        }        
        return ClosestFromTheCenter;
    }


    private void ResetLockOn()
    {
        if (FollowHandlerGO != null && FollowHandlerGO.activeSelf)
            FollowHandlerGO.SetActive(false);
        //Debug.Log("DEACTIVEATELOCKON: " + ScreenInteractables.Count);

        PlayerCameraManager.DeactivateLockOn();
        CurrentLockOnTarget = null;
        AllowRecenter();
        lockedOn=false;
    }

    private void AllowRecenter()
    {
        foreach (var rScripts in RecenterCameraScripts)
        {
            rScripts.CanRecenter = true;
        }
    }
    private void DisallowRecenter()
    {
        foreach (var rScripts in RecenterCameraScripts)
        {
            rScripts.CancelRecentering();
            rScripts.CanRecenter = false;
        }
    }

    protected override Vector3 GetOrigin()
    {
        return Origin.position+OriginOffset;
    }
}
