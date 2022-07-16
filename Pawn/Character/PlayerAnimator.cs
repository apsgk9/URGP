using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField]
    CharacterAnimatorNamingList AnimParam;

    [SerializeField]
    Animator Animator;
    [SerializeField]
    Character _characterStateMachine;
    private Vector3 _previousMovmementAxis;
    [SerializeField]
    private float UpdateRate=0.1f;
    private float CurrentTimeUpdate;
    private float currentDelta;


    private void Awake()
    {
        Animator = GetComponent<Animator>();
        if (_characterStateMachine == null)
        {
            FindPlayerStateMachine();
        }
        CurrentTimeUpdate=Time.time;
        currentDelta=0;
    }
    private void OnEnable()
    {
        _characterStateMachine.TriggerJump += SetJumpTrigger;
        
    }
    private void OnDisable()
    {
        _characterStateMachine.TriggerJump -= SetJumpTrigger;
        
    }

    private void SetJumpTrigger()
    {
        Animator.SetBool(AnimParam.JumpTriggerHash,true);
    }
    
    private void Update()
    {
        HandleAnimator();
    }

    private void HandleAnimator()
    {
        Animator.SetInteger(AnimParam.LocomotionModeHash, (int)_characterStateMachine.LocomotionMode);
        Animator.SetInteger(AnimParam.AirborneModeHash, (int)_characterStateMachine.AirborneMode);

        Animator.SetBool(AnimParam.MovementPressedHash, _characterStateMachine.IsMovementPressed);
        Animator.SetBool(AnimParam.UsingControllerHash, InputHelper.DeviceInputTool.IsUsingController());
        Animator.SetBool(AnimParam.IsJumpingHash, _characterStateMachine.IsJumping);


        Animator.SetFloat(AnimParam.ControllerDeltaHash, ReturnDeltaMovement());
    }

    private void FindPlayerStateMachine()
    {
        _characterStateMachine = GetComponentInParent<Character>();
        if (_characterStateMachine == null)
        {
            Debug.LogError("PlayerAnimator cannot find PlayerStateMachine");
        }
    }

    private void OnValidate()
    {
        Animator = GetComponent<Animator>();
        _characterStateMachine = GetComponentInParent<Character>();
    }

    private float ReturnDeltaMovement()
    {
        if(Time.time-CurrentTimeUpdate>UpdateRate)
        {
            CurrentTimeUpdate=Time.time;
            return CalculateMovementDelta();
        }
        return currentDelta;

    }

    private float CalculateMovementDelta()
    {
        var movementAxis = _characterStateMachine.MovementInput;
        currentDelta = (movementAxis - _previousMovmementAxis).magnitude;

        _previousMovmementAxis = movementAxis;
        return currentDelta;
    }
}
