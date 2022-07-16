using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LocomotionEnmus;
/*
    Describes what what movement is allowed for the charactercontroller
*/
public enum MovementState
{
    Locomotion, //Can Move on its own
    NonLocomotion,  //NoMovement, Let physics do its own thing
    Manual  //All movement is done externally.
}
public class CharacterAction : Character
{    
    public event Action OnGroundedAction;
    public bool TriggerAction;
    public bool LocomotionStateMachineCanTransition {get {return _LocomotionStateMachineCanTransition;} set{_LocomotionStateMachineCanTransition=value;}}
    protected NonLocomotionBaseState _currentNonLocomotionState;

    public NonLocomotionBaseState CurrentNonLocomotionState { get { return _currentNonLocomotionState; } set { _currentNonLocomotionState = value; } }
    
    protected NonLocomotionStateFactory _nonLocomotionstates;
    public bool _NonLocomotionStateMachineCanTransition=true;
    public MovementState CharacterMovementState;

    protected override void Awake()
    {
        base.Awake();

        _nonLocomotionstates = new NonLocomotionStateFactory(this);
        CharacterMovementState=MovementState.Locomotion;
        
    }
    protected override void Start()
    {
        base.Start();

        _currentNonLocomotionState = _nonLocomotionstates.Grounded();
        _currentNonLocomotionState.Enter();        
    }

    protected override void FixedUpdate()
    {
        HandleCharacterState();
    }
    protected override void Update()
    {
        base.Update();
        if (CanDoGroundedAction())
        {
            //_characterInput.ResetAttack();
            TriggerAction=false;
            OnGroundedAction?.Invoke();
        }
    }

    private void HandleCharacterState()
    {
        switch (CharacterMovementState)
        {
            case MovementState.Locomotion:
                Locomotion();
                break;
            case MovementState.NonLocomotion:
                NonLocomotion();
                break;
            case MovementState.Manual:
                ManualMovement();
                break;
        }
    }

    private void ManualMovement()
    {
        _Mover.SetVelocity(SavedMomentum + Velocity);
    }

    private void NonLocomotion()
    {
        _allowMovement = false;
        CanRotate = false;
        _allowJumping = false;
        HandleCharacterLocomotionMotion(_currentNonLocomotionState);
    }

    private void Locomotion()
    {
        _allowMovement = true;
        CanRotate = true;
        _allowJumping = true;
        HandleCharacterLocomotionMotion(_currentLocomotionState);
    }

    

    private bool CanDoGroundedAction()
    {
        return TriggerAction && _airborneMode == AirborneMode.Grounded && IsJumping == false;
    }

    public void ForceSwitchState(LocomotionMode newLocomotionMode)
    {
        if(_locomotionMode==newLocomotionMode)
            return;


        _locomotionMode=newLocomotionMode;

        bool previousLocomotionStateMachineCanTransition=LocomotionStateMachineCanTransition;
        LocomotionStateMachineCanTransition=true;

        if(newLocomotionMode!=LocomotionMode.Idle)
        {
            _forceMovementPressed=true;
        }

        if(_airborneMode!=AirborneMode.Grounded)
        {
            _currentLocomotionState.Exit(); //Exit fromairborne
            _currentLocomotionState = _Locomotionstates.Grounded();
            _currentLocomotionState.Enter();  //Force to grounded
        }

        _currentLocomotionState.UpdateStates();

        LocomotionStateMachineCanTransition=previousLocomotionStateMachineCanTransition;
        _forceMovementPressed=false;
    }


    //Todo flesh this out later. right now doesn't take into account on anything
    public void ForceSwitchState(AirborneMode airborneMode)
    {
        bool temp=LocomotionStateMachineCanTransition;
        LocomotionStateMachineCanTransition=true;

        _currentLocomotionState.Exit();
        _airborneMode=airborneMode;
        _currentLocomotionState = _Locomotionstates.Airborne();
        _currentLocomotionState.Enter();

        
        _currentLocomotionState.UpdateStates();

        LocomotionStateMachineCanTransition=temp;
    }

    public void DisableJumping()
    {
        _allowJumping=false;
    }
    public void EnableJumping()
    {
        _allowJumping=true;
    }

    public override void HaltCharacterController()
    {
        base.HaltCharacterController();

        TriggerAction=false;
    }
}

