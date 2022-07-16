using System.Collections;
using System.Collections.Generic;
using Movement;
using UnityEngine;
using static LocomotionEnmus;

public class NonLocomotionGroundedState : NonLocomotionBaseState
{

    public NonLocomotionGroundedState(CharacterAction currentContext, NonLocomotionStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory)
    {        
        IsRootState = true;
        InitializeSubState();
    }

    protected override void EnterState()
    {
        Ctx.AirborneMode=AirborneMode.Grounded;
        Ctx.LocomotionMode=LocomotionMode.Idle;
        Ctx.IsJumping = false;
        
        //Prevent Momentum Caused by falling from transferring to next Jump. Therefore faster jumps
        //Heh, could have been a movement technique
        bool isSliding = Ctx.IsGroundTooSteep(Ctx.Mover.GetGroundNormal(), Ctx.CharacterGameObject, Ctx.SlopeLimit);
        if(!isSliding)
        {
            Ctx.SavedMomentum=Vector3.zero;
        }
    }


    public override void UpdateState()
    {
        EnterCheckSwitchStates();
        if (IsActive == false)
            return;
        HandleMomentum();
        IdleVelocity();
    }

    private void IdleVelocity()
    {
        if (Ctx.ShouldUseRootMotion())
        {
            Ctx.ApplyRootMotion();
        }
        else
        {
            Ctx.Velocity = Vector3.zero;
        }
    }

    private void HandleMomentum()
    {
        bool isSliding = Ctx.IsGroundTooSteep(Ctx.Mover.GetGroundNormal(), Ctx.CharacterGameObject, Ctx.SlopeLimit);

        //Momentum-------------

        Vector3 momentum = Ctx.SavedMomentum;

        //Apply friction and gravity to 'momentum';
        momentum = Ctx.Momentum(Ctx.CharacterGameObject, momentum, Ctx.YGravity, Ctx.GroundFriction);

        if (isSliding)
        {
            momentum += Ctx.SlidingMomentum(Ctx.CharacterGameObject, momentum, Ctx.Mover.GetGroundNormal(), Ctx.SlideGravity, Ctx.SlidingMaxVelocity);
        }

        //-------------------------

        Ctx.SavedMomentum = momentum;
    }

    protected override void ExitState()
    {
    }

    public override void InitializeSubState()
    {

    }

    public override void CheckSwitchStates()
    {

        if (!Ctx.Mover.IsGrounded())
        {
            SwitchState(Factory.Airborne());
        }
    }
}

