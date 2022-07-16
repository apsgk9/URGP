using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtilityFunctions;
using static LocomotionEnmus;
/*
    Only Does a transition to a state and then back.
    Locks the Character State then free's it when it can be cancelled

    //Assumes grounded
*/
public class BasicCharacterAction : CharacterAnimatorMonoBehaviour<CharacterAction>
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
    
    [SerializeField]
    AnimatorLocomotionTransition EndingTransition;
    protected override void Awake()
    {
        base.Awake();        
        FindCharacterAdvanceAnimator();
        transitioned = true;
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
        else
        {
            if (transitioned == false)
            {
                ExitAction();
            }
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
        _characterController.CharacterMovementState=MovementState.Locomotion;
        if (ExitSettings.FixedDuration)
        {
            _AdvanceAnimator.FadeAnimator(ExitSettings.TimeDuration);
        }
        else
        {
            _AdvanceAnimator.FadeAnimator(ExitSettings.TimeDuration);
        }
        HandleChangeWithCharacterLocomotionModeEnd();
        _Animator.Play(EndingTransition.StateToTransitionToAtEnd, EndingTransition.Layer);

        transitioned = true;
    }

    private void ResumeNormalBehaviour()
    {
        //Kinda dangerouse to have only this to determine if a player can rotate     
        _CanCancelLock = false;
        _characterController.EnableJumping();

        //_characterController.LocomotionStateMachineCanTransition = true;

        _characterController.CanRotate=true;
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
        _characterController.CanRotate=true;
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
        
        //Character Controller
        HandleChangeWithCharacterLocomotionModeStart();
        
        //_characterController.LocomotionStateMachineCanTransition = false;
        _characterController.CharacterMovementState=MovementState.NonLocomotion;

        _characterController.DisableJumping();
        _characterController.CanRotate=false;


    }

    private void HandleChangeWithCharacterLocomotionModeStart()
    {
        if (EndingTransition.ChangeStart)
        {
            if (!EndingTransition.StartLocomotionModeSwitch.Toggle)
            {
                _characterController.ForceSwitchState(EndingTransition.StartLocomotionModeSwitch.LocomotionMode);
            }
            else
            {
                _characterController.ForceSwitchState(EndingTransition.StartLocomotionModeSwitch.AirborneMode);
            }
        }
    }
    private void HandleChangeWithCharacterLocomotionModeEnd()
    {
        

        if (EndingTransition.ChangeEnd)
        {
            if (!EndingTransition.EndLocomotionModeSwitch.Toggle)
            {
                _characterController.ForceSwitchState(EndingTransition.EndLocomotionModeSwitch.LocomotionMode);
            }
            else
            {
                _characterController.ForceSwitchState(EndingTransition.EndLocomotionModeSwitch.AirborneMode);
            }
        }
    }
    
    

}
