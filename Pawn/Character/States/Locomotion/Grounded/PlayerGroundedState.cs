using System;
using System.Collections;
using System.Collections.Generic;
using Movement;
using UnityEngine;
using static LocomotionEnmus;

public class PlayerGroundedState : PlayerLocomotionBaseState
{

    public PlayerGroundedState(Character currentContext, PlayerLocomotionStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory)
    {
        IsRootState = true;
        InitializeSubState();
    }

    protected override void EnterState()
    {
        Ctx.AirborneMode=AirborneMode.Grounded;
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
        HandleJump();
        EnterCheckSwitchStates();
        if (IsActive == false)
            return;

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

        if (Ctx.IsMovementPressed && Ctx.LocomotionMode == LocomotionMode.Walk)
        {
            SetSubState(Factory.Walk());
        }
        else if (Ctx.IsMovementPressed && Ctx.LocomotionMode == LocomotionMode.Run)
        {
            SetSubState(Factory.Run());
        }
        else if (Ctx.IsMovementPressed && Ctx.LocomotionMode == LocomotionMode.Sprint)
        {
            SetSubState(Factory.Sprint());
        }
        else
        {
            SetSubState(Factory.Idle());
        }

    }

    public override void CheckSwitchStates()
    {

        if (!Ctx.Mover.IsGrounded() || Ctx.IsJumping)
        {
            SwitchState(Factory.Airborne());
        }
    }
    private void HandleJump()
    {
        if (Ctx.IsJumpPressed == false) //Jump Button is Released
        {
            Ctx.JumpLock = false;
        }
        else //Jump is Pressed
        {
            if (Ctx.JumpLock == false)
            {                
                Jump();
            }

        }
    }

    private void Jump()
    {
        Ctx.JumpLock = true;
        Ctx.IsJumping = true;
        Ctx.InvokeJump();

        Ctx.SavedMomentum = VectorMath.RemoveDotVector(Ctx.SavedMomentum, Ctx.CharacterGameObject.transform.up);
        Ctx.SavedMomentum += Ctx.CharacterGameObject.transform.up * Ctx.JumpSpeed;
        Ctx.CurrentJumpStartTime = Time.time;
        LogState("JUMP");
    }
}
