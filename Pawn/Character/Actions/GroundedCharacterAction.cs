using UnityEngine;
using UtilityFunctions;
using static LocomotionEnmus;

public class GroundedCharacterAction : CharacterAnimatorMonoBehaviour<CharacterAction>
{
    [SerializeField]
    protected AdvanceAnimator _AdvanceAnimator;
    [Header("Crossfade Into Action")]
    public AnimationClip AnimationClip;
    public float ActionFadeFixedTime=0.1f;
    public float ActionCancellableTime=0.5f;    
    public float TimeWhenActionCanbeCancelled;

    public float TimeWhenExitShouldHappen { get; private set; }

    private bool _CanCancelLock;
    private bool transitioned = true;
    
    [SerializeField]
    TransitionSettings ExitSettings;
    
    
    [Header("Exit Transition")]
    [SerializeField]
    public AnimatorStateList StateToTransitionToAtEnd;
    public AnimatorStateList FallingState;
    public bool ChangeStart=false;
    public LocomotionMode  StartLocomotionMode=LocomotionMode.Idle;
    public bool ChangeEnd=false;
    public LocomotionMode  EndLocomotionMode=LocomotionMode.Idle;
    protected override void Awake()
    {
        base.Awake();        
        FindCharacterAdvanceAnimator();
        transitioned = true;
    }
    private void OnValidate()
    {
        if(_Animator)
        {
            StateToTransitionToAtEnd.Animator=_Animator;
            
        }
    }
    private void FindCharacterAdvanceAnimator()
    {
        if(_AdvanceAnimator==null)
        {
            _AdvanceAnimator= ComponentUtil.FindComponentWithinGameObject<AdvanceAnimator>(gameObject);
        }
    }

    

    private void Start()
    {
        TimeWhenActionCanbeCancelled=Time.time;
    }

    private void Update()
    {
        if (GameState.isPaused)
            return;

        if (_characterController.IsGrounded())
        {
            GroundedBehaviour();
        }
        else //interrupt action
        {
            UngroundedBehaviour();
        }

    }

    private void UngroundedBehaviour()
    {
        if (_CanCancelLock)
        {
            ResumeNormalBehaviour();
        }
        if (transitioned == false)
        {
            
            ExitAction();
        }
    }

    private void GroundedBehaviour()
    {
        if (!CanBeCancelled())
            return;

        if (CanBeCancelled() && _CanCancelLock)
        {
            ResumeNormalBehaviour();
        }

        if (transitioned == false)
        {
            if (TimeWhenExitShouldHappen < Time.time)
            {
                ExitAction();
            }
        }
    }

    private void ExitAction()
    {
        transitioned = true;
        if (ExitSettings.FixedDuration)
        {
            _AdvanceAnimator.FadeAnimator(ExitSettings.TimeDuration);
        }
        else
        {
            _AdvanceAnimator.FadeAnimator(ExitSettings.TimeDuration);
        }
        if(_characterController.IsGrounded())
        {
            HandleChangeWithCharacterLocomotionModeEnd();
            _Animator.Play(StateToTransitionToAtEnd.Name, StateToTransitionToAtEnd.Layer);
        }
        else
        {
            SuddenLossofGround();

        }

    }

    private void SuddenLossofGround()
    {
        _Animator.Play(FallingState.Name , FallingState.Layer);
        
        _characterController.SavedMomentum = 
        Vector3.ClampMagnitude(_characterController.SavedMomentum/2,_characterController.SprintSpeed);
    }

    private void ResumeNormalBehaviour()
    { 
        _CanCancelLock = false;
        _characterController.CharacterMovementState=MovementState.Locomotion;
    }



    private bool CanBeCancelled()
    {
        return TimeWhenActionCanbeCancelled < Time.time;
    }

    private void OnEnable()
    {
        _characterController.OnGroundedAction += TriggerAction;    
        _characterController.TriggerJump +=PlayerHasJumped;    
    }

    private void OnDisable()
    {
        _characterController.OnGroundedAction -= TriggerAction;     
        _characterController.TriggerJump -=PlayerHasJumped;   
    }
    private void PlayerHasJumped()
    {
        _characterController.CharacterMovementState=MovementState.Locomotion;
    }
    
    private void TriggerAction()
    {
        if (GameState.isPaused || TimeWhenActionCanbeCancelled > Time.time)
            return;


        _AdvanceAnimator.FadeClip(AnimationClip,ActionFadeFixedTime);

        transitioned=false;

        TimeWhenActionCanbeCancelled = Time.time + ActionCancellableTime;
        TimeWhenExitShouldHappen = Time.time + ExitSettings.ExitTime*AnimationClip.length;

        transitioned = false;
        _CanCancelLock = true;
        
        HandleChangeWithCharacterLocomotionModeStart();
        _characterController.CharacterMovementState=MovementState.NonLocomotion;



    }

    private void HandleChangeWithCharacterLocomotionModeStart()
    {
        if (ChangeStart)
        {
            _characterController.ForceSwitchState(StartLocomotionMode);
        }
    }
    private void HandleChangeWithCharacterLocomotionModeEnd()
    {       

        if (ChangeEnd)
        {
            _characterController.ForceSwitchState(EndLocomotionMode);  
        }
    }
}
