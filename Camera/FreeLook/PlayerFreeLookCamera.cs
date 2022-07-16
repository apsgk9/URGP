using Cinemachine;
using UnityEngine;
using System.Linq;
using MainObject;
using System;
using System.Collections.Generic;
using System.Collections;

public class PlayerFreeLookCamera : CameraPlayer
{
    private const float CUTOFFTHRESHOLDFORDAMPING = 50f;
    [Header("PlayerFreeLookCamera")]
    public CinemachineTargetGroup CinemachineTargetGroup;
    public CinemachineFreeLook FreeLookVirtualCam;
    public static PlayerFreeLookCamera CurrentPlayerFreeLookCamera;

    //The Float part is time it has.

    [SerializeField]
    private Dictionary<Transform,float> TargetsToSlowlyAdd;
    [SerializeField]
    private Dictionary<Transform,float> TargetsFinished;

    [SerializeField]
    private Dictionary<Transform,float> TargetsToSlowlyRemove;

    [Min(0)] [SerializeField]
    private float _focusDuration =0.5f;
    public float FocusDuration{get {return _focusDuration;} set{ _focusDuration= Mathf.Max(0,value);}}

    
    [Min(0)] [SerializeField]
    private float _slowdownFocusDuration =5f;
    public float SlowdownFocusDuration{get {return _slowdownFocusDuration;} set{ _slowdownFocusDuration= Mathf.Max(0,value);}}


    private bool _pauseHandlingTargetWeights=false;

    //---------------- DAMPING SETTINGS
    public float BlendingDampMultiplier =1f;
    private CinemachineComposer _middleComposer;
    private CinemachineOrbitalTransposer _middleOrbital;
    private float _middleInitialVerticalDamping;
    private float _middleInitialHorizontalDamping;
    private float _middleInitialXDamping;
    private float _middleInitialYDamping;
    private float _middleInitialZDamping;
    private bool _setToLowerValueDamping;
    private bool _setToInitialDamping;
    private Vector3 _CinemachineTargetGroupDisplacement;
    private Vector3 _previousCinemachineTargetGroupPosition;
    [SerializeField] [ReadOnly]
    private float _CinemachineTargetGroupVelocity;
    [SerializeField]
    private bool _UseSquareMagnitude;
    //Collider
    [SerializeField]
    private CinemachineCollider _cinemachineCollider;

    private float _cinemachineColliderOriginalSmoothingTime;
    //public bool IgnoreRealtimeTime;



    //For Now assume its only one and is for initialization
    protected override void Awake()
    {
        base.Awake();
        if (FreeLookVirtualCam)
        {
            FreeLookVirtualCam.enabled = false;
        }
        SetupTargets();
        SetupDamping();

        _cinemachineCollider= FreeLookVirtualCam.GetComponent<CinemachineCollider>();
        _cinemachineColliderOriginalSmoothingTime= _cinemachineCollider.m_SmoothingTime;

    }

    //public void SetIgnoreTime(bool inputIgnore)
    //{
    //    IgnoreRealtimeTime=inputIgnore;
    //}

    private void SetupTargets()
    {
        CinemachineTargetGroup.m_Targets = new CinemachineTargetGroup.Target[0];

        //To Prevent jittering when constantly adding and removing targets. Group Center changes the value depending on target priority
        CinemachineTargetGroup.m_PositionMode= CinemachineTargetGroup.PositionMode.GroupAverage; 

        TargetsToSlowlyAdd = new Dictionary<Transform, float>();
        TargetsFinished = new Dictionary<Transform, float>();
        TargetsToSlowlyRemove = new Dictionary<Transform, float>();
    }

    private void SetupDamping()
    {
        _middleComposer = FreeLookVirtualCam.GetRig(1).GetCinemachineComponent<CinemachineComposer>();
        _middleOrbital = FreeLookVirtualCam.GetRig(1).GetCinemachineComponent<CinemachineOrbitalTransposer>();

        _middleInitialVerticalDamping= _middleComposer.m_VerticalDamping;
        _middleInitialHorizontalDamping= _middleComposer.m_HorizontalDamping;

        _middleInitialXDamping= _middleOrbital.m_XDamping;
        _middleInitialYDamping= _middleOrbital.m_YDamping;
        _middleInitialZDamping= _middleOrbital.m_ZDamping;

        _setToInitialDamping=true;
        _setToLowerValueDamping=false;

        _previousCinemachineTargetGroupPosition=CinemachineTargetGroup.transform.position;
    }

    public override void SetMainFocusTo(Transform target)
    {
        var oldFocus=MainTransformToFocus;
        var newFocus= base.HandleNewMainFocusedTarget(target);

        AssignNewMainToFreeLookCamera(oldFocus,newFocus);
        //Enables this if it isn't already enabled
        FreeLookVirtualCam.enabled = true;
    }

    private void AssignNewMainToFreeLookCamera(Transform oldFocus,Transform newFocus)
    {
        RemoveTargetSoft(oldFocus);
        AddTargetSoft(newFocus);
    }

    //TODO- Maybe add a custom blend rather than the default one
    public void AddTargetSoft(Transform target)
    {
        bool FoundInTargetGroup = CinemachineTargetGroup.FindMember(target) != -1;
        if (!FoundInTargetGroup)
        {
            if(CinemachineTargetGroup.m_Targets.Count()==0)
            {
                CinemachineTargetGroup.AddMember(target, 1f, 1f);
                TargetsFinished.Add(target,GetFocusDuration());
            }
            else
            {
                CinemachineTargetGroup.AddMember(target, 0f, 1f);
                TargetsToSlowlyAdd.Add(target,0);
            }
            
        }
        else
        {
            if(TargetsToSlowlyRemove.ContainsKey(target))
            {
                TargetsToSlowlyAdd.Add(target,TargetsToSlowlyRemove[target]);
                TargetsToSlowlyRemove.Remove(target);
            }
            else
            {
                Debug.LogError("TARGET CANNOT BE ADDED");                
            }
        }
    }

    public bool DoesTargetExist(Transform target)
    {
        return CinemachineTargetGroup.FindMember(target) != -1;
    }
    public void RemoveTarget(Transform target)
    {
        bool FoundInTargetGroup = DoesTargetExist(target);
        if (FoundInTargetGroup)
        {
            CinemachineTargetGroup.RemoveMember(target);
        }
    }
    public void AddTarget(Transform target)
    {
        bool FoundInTargetGroup = DoesTargetExist(target);
        if (!FoundInTargetGroup)
        {
            CinemachineTargetGroup.AddMember(target,1,1);
        }
    }
    public void RemoveTargetSoft(Transform target)
    {
        bool FoundInTargetGroup = DoesTargetExist(target);
        if (FoundInTargetGroup)
        {
            float value;
            if(TargetsToSlowlyAdd.ContainsKey(target))
            {
                value=TargetsToSlowlyAdd[target];
                TargetsToSlowlyAdd.Remove(target);
            }
            else if(TargetsFinished.ContainsKey(target))
            {                
                value=TargetsFinished[target];
                TargetsFinished.Remove(target);          
            }
            else
            {
                Debug.LogError("TARGET CANNOT BE FOUND");
                return;
            }

            TargetsToSlowlyRemove.Add(target,value);
            int f=CinemachineTargetGroup.FindMember(target);
            
        }
    }
    
    protected override void Update()
    {
        base.Update();
        if (GameState.isPaused)
            return;


        HandleCollider();

        HandleCinemachineTargetSpeed();

        HandleDampingWhenBlending();

        HandleTargetWeights();

    }


    //Prevents Collision when transitioning
    private void HandleCollider()
    {
        if(_cinemachineCollider==null)
            return;
        
        if(Time.timeScale!=1.0f)
        {
            _cinemachineCollider.m_SmoothingTime= _cinemachineColliderOriginalSmoothingTime*Time.timeScale;
        }
        else if(_cinemachineColliderOriginalSmoothingTime!=_cinemachineCollider.m_SmoothingTime)
        {
            _cinemachineCollider.m_SmoothingTime = _cinemachineColliderOriginalSmoothingTime;
        }

    }
    /*
        Realtime
    */
    private void HandleCinemachineTargetSpeed()
    {
        _CinemachineTargetGroupDisplacement = CinemachineTargetGroup.transform.position - _previousCinemachineTargetGroupPosition;
        _previousCinemachineTargetGroupPosition = CinemachineTargetGroup.transform.position;
        float magnitude = 0;
        if (_UseSquareMagnitude)
        {
            magnitude = _CinemachineTargetGroupDisplacement.sqrMagnitude;
        }
        else
        {
            magnitude = _CinemachineTargetGroupDisplacement.magnitude;
        }
        float finalMagnitude = (magnitude < 0.0001f) ? 0 : magnitude;
        _CinemachineTargetGroupVelocity = finalMagnitude / GetDeltaTime();
    }

    private float GetDeltaTime()
    {
        return Time.unscaledDeltaTime;
        //if(IgnoreRealtimeTime)
        //{
        //    return Time.unscaledDeltaTime;
        //}
        //return Time.deltaTime;
    }

    private void HandleDampingWhenBlending()
    {
        if (IsBlending())
        {
            SetMiddleDampingToLowerValue();
            if(!_setToLowerValueDamping)
            {
                _setToInitialDamping=false;
                _setToLowerValueDamping=true;
            }
        }
        else
        {
            if(!_setToInitialDamping)
            {
                SetMiddleDampingToInitial();
                _setToInitialDamping=true;
                _setToLowerValueDamping=false;
            }
        }
    }

    private void SetMiddleDampingToLowerValue()
    {
        float toDivide= Mathf.Clamp(_CinemachineTargetGroupVelocity*BlendingDampMultiplier,1,Mathf.Infinity);
        if(toDivide> CUTOFFTHRESHOLDFORDAMPING)
        {
            toDivide=Mathf.Infinity;
        }
        _middleComposer.m_VerticalDamping = _middleInitialVerticalDamping/toDivide;
        _middleComposer.m_HorizontalDamping = _middleInitialVerticalDamping/toDivide;

        _middleOrbital.m_XDamping = _middleInitialXDamping/(toDivide*2);
        _middleOrbital.m_YDamping = _middleInitialYDamping/(toDivide*2);
        _middleOrbital.m_ZDamping = _middleInitialZDamping/(toDivide*2);
    }

    private void SetMiddleDampingToInitial()
    {
        _middleComposer.m_VerticalDamping = _middleInitialVerticalDamping;
        _middleComposer.m_HorizontalDamping = _middleInitialVerticalDamping;

        _middleOrbital.m_XDamping = _middleInitialXDamping;
        _middleOrbital.m_YDamping = _middleInitialYDamping;
        _middleOrbital.m_ZDamping = _middleInitialZDamping;
    }

    private bool IsBlending()
    {
        return TargetsToSlowlyAdd.Keys.Count != 0 || TargetsToSlowlyRemove.Keys.Count != 0;
    }

    private void HandleTargetWeights()
    {
        if(_pauseHandlingTargetWeights) //Pause if not main
            return;

        var toAddKeys = new List<Transform>(TargetsToSlowlyAdd.Keys);

        foreach (var toAdd in toAddKeys)
        {

            //Get Member
            int memberIndex = CinemachineTargetGroup.FindMember(toAdd);
            CinemachineTargetGroup.Target member = CinemachineTargetGroup.m_Targets[memberIndex];


            //Calculate Percent
            TargetsToSlowlyAdd[toAdd] += GetDeltaTime();
            float percent = Mathf.Clamp(TargetsToSlowlyAdd[toAdd] / GetFocusDuration(), 0, 1);

            //Set New Weight
            float newWeight = Mathf.SmoothStep(0, 1, percent);
            CinemachineTargetGroup.m_Targets[memberIndex].weight = newWeight;

            //Handle if done
            if (percent >= 1 || CinemachineTargetGroup.m_Targets.Count()==1)
            {
                TargetsFinished.Add(toAdd, GetFocusDuration() );
                TargetsToSlowlyAdd.Remove(toAdd);
                CinemachineTargetGroup.m_Targets[memberIndex].weight = 1;
            }
        }

        var toRemoveKeys = new List<Transform>();
        if (TargetsToSlowlyRemove.Keys.Count > 0)
        {
            toRemoveKeys.AddRange(TargetsToSlowlyRemove.Keys);
        }


        foreach (var toRemove in toRemoveKeys)
        {
            //Get Member
            int memberIndex = CinemachineTargetGroup.FindMember(toRemove);
            CinemachineTargetGroup.Target member = CinemachineTargetGroup.m_Targets[memberIndex];

            //Calculate Percent
            TargetsToSlowlyRemove[toRemove] -= GetDeltaTime();
            float percent = Mathf.Clamp(TargetsToSlowlyRemove[toRemove] / GetFocusDuration(), 0, 1);

            //Set New Weight
            float newWeight = Mathf.SmoothStep(0, 1, percent);
            CinemachineTargetGroup.m_Targets[memberIndex].weight = newWeight;
            //Handle if done
            if (newWeight <= 0)
            {
                TargetsToSlowlyRemove.Remove(toRemove);
                CinemachineTargetGroup.RemoveMember(member.target);
            }
        }
    }

    private float GetFocusDuration()
    {
        //if(IgnoreRealtimeTime)
        //{
        //    return SlowdownFocusDuration;
        //}
        return FocusDuration;
    }

    //Seems like its still going even if I turn it off
    public override void  HandleBeingMainCamPlayer()
    {
        //Debug.Log(gameObject.name+ " is now main camera.");
        base.HandleBeingMainCamPlayer();
        FreeLookVirtualCam.Priority=Priority+1;
        if(CurrentPlayerFreeLookCamera!=null)
        {
            FreeLookVirtualCam.m_XAxis = CurrentPlayerFreeLookCamera.FreeLookVirtualCam.m_XAxis;
            FreeLookVirtualCam.m_YAxis = CurrentPlayerFreeLookCamera.FreeLookVirtualCam.m_YAxis;
        }
        CurrentPlayerFreeLookCamera=this;
        FreeLookVirtualCam.enabled=true;

        _pauseHandlingTargetWeights=false;
    }

    public override void  HandleBeingOffstageCamPlayer()
    {
        if(!isMainCameraAssociatedLockOnCamera())
        {
            _pauseHandlingTargetWeights=true;
        }
        //Debug.Log(gameObject.name+ " is now offstage camera.");
        base.HandleBeingOffstageCamPlayer();
        FreeLookVirtualCam.Priority=Priority;

       if(CameraManager.Instance && CameraManager.Instance.PlayerCameraManagerReference)
            CameraManager.Instance.PlayerCameraManagerReference.FinishedBlending+=PlayerVirtualCameraDoneBlending;
        
        

        FreeLookVirtualCam.enabled=false;
    }

    private void RemoveAdditionalTargets()
    {
        Debug.Log("CLEAR TARGETS");
        ClearTargets(ref TargetsToSlowlyAdd);
        ClearTargets(ref TargetsFinished);
        ClearTargets(ref TargetsToSlowlyRemove);
    }
    private void ClearTargets(ref Dictionary<Transform,float> Targets)
    {
         if(Targets!=null && Targets.Count>0)
        {
            bool mainFound=false;
            float valMain=0.0f;
            foreach(var tgt in Targets.Keys)
            {
                //Do not remove current main
                if(tgt==MainTransformToFocus)
                {
                    mainFound=true;
                    valMain=Targets[tgt];
                    continue;
                }
                CinemachineTargetGroup.RemoveMember(tgt);
            }
            Targets.Clear();
            if(mainFound)
            {
                Targets.Add(MainTransformToFocus,valMain);
            }
        }
    }

    //This assumes its not from a main cameraplayer
    private void PlayerVirtualCameraDoneBlending()
    {

        //To Prevent Target Removal when locking On
        bool isAssociatedLockOnCamera= isMainCameraAssociatedLockOnCamera();

        if (!isAssociatedLockOnCamera)
        {
            RemoveAdditionalTargets();
        }

        if(CameraManager.Instance && CameraManager.Instance.PlayerCameraManagerReference)
            CameraManager.Instance.PlayerCameraManagerReference.FinishedBlending-=PlayerVirtualCameraDoneBlending;
    }
    private bool isMainCameraAssociatedLockOnCamera()
    {
        if(CameraPlayer.Main is PlayerLockOnCamera)
        {
            var lockOn=CameraPlayer.Main as PlayerLockOnCamera;
            if(lockOn.GetAssociatedPlayerFreeLookCamera()==this)
            {
                return true;
            }
        }
        return false;
    }

    protected override void OnValidate()
    {
        
        if(FreeLookVirtualCam)
        {
            FreeLookVirtualCam.Priority=Priority;
        }
        base.OnValidate();
        
        
    }
    
}
