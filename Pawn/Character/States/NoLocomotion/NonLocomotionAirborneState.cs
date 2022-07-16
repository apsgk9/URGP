using System.Collections;
using System.Collections.Generic;
using Movement;
using UnityEngine;
using static LocomotionEnmus;

public class NonLocomotionAirborneState : NonLocomotionBaseState
{

    public NonLocomotionAirborneState(CharacterAction currentContext, NonLocomotionStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory)
    {        
        IsRootState = true;
        InitializeSubState();
    }

    protected override void EnterState()
    {
        Ctx.OnGroundContactLost();
    }

    public override void UpdateState()
    {
        EnterCheckSwitchStates();
        
        Vector3 momentum = Ctx.SavedMomentum;

        momentum = Ctx.Momentum(Ctx.CharacterGameObject, momentum, Ctx.YGravity, Ctx.AirFriction);

        momentum.y= Mathf.Clamp(momentum.y,-Ctx.MaxVerticalSpeed,Ctx.MaxVerticalSpeed);

        Ctx.SavedMomentum = momentum;

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

    protected override void ExitState()
    {
    }

    public override void InitializeSubState()
    {
        if(Ctx.Mover.GetVelocity().y>0)
        {
            SetSubState(Factory.Rising());
        }
        else if(Ctx.Mover.GetVelocity().y<=0)
        {
            SetSubState(Factory.Falling());
        }
    }

    public override void CheckSwitchStates()
    {
        if (Ctx.Mover.IsGrounded() && !Ctx.IsJumping) //IsJumping to prevent being grounded when trying to jump
        {
            Ctx.AirborneMode=AirborneMode.Grounded;
            SwitchState(Factory.Grounded());
        }
    }
}

