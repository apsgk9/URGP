using System.Collections;
using System.Collections.Generic;
using Movement;
using UnityEngine;

//Kinda not named well but just like a modifier to the super state so it doesn't get clogged
public class PlayerJumpState : PlayerLocomotionBaseState
{

    public PlayerJumpState(Character currentContext, PlayerLocomotionStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory){}

    protected override void EnterState()
    {
        Ctx.AirborneMode=LocomotionEnmus.AirborneMode.Jumping;
        Ctx.IsJumping=true;
    }

    public override void UpdateState()
    {
        Ctx.IsJumping= (Time.time - Ctx.CurrentJumpStartTime) < Ctx.JumpDuration;
        EnterCheckSwitchStates();
        if(!IsActive)
            return;

        if (!Ctx.IsJumping)
            return;
            
        Ctx.SavedMomentum = HandleJumping(Ctx.SavedMomentum);
    }



    protected override void ExitState()
    {
        Ctx.IsJumping=false;
    }

    public override void InitializeSubState()
    {
    }

    public override void CheckSwitchStates()
    {
        if(Ctx.CeilingDetector!=null && Ctx.CeilingDetector.HitCeiling())
        {
            Ctx.IsJumping=false;
			Ctx.SavedMomentum = VectorMath.RemoveDotVector(Ctx.SavedMomentum, Ctx.CharacterGameObject.transform.up);
            SwitchState(Factory.Falling());
            return;
        }

        //Only Check when the jump duration is done
        if(Ctx.IsJumping)
            return;
        if(Ctx.Mover.GetVelocity().y>0)
        {            
            SwitchState(Factory.Rising());
        }
        else if(Ctx.Mover.GetVelocity().y<0)
        {
            SwitchState(Factory.Falling());
        }
    }

    private Vector3 HandleJumping(Vector3 momentum)
    {
        momentum = VectorMath.RemoveDotVector(momentum, Ctx.CharacterGameObject.transform.up);
        momentum += Ctx.CharacterGameObject.transform.up * Ctx.JumpSpeed;

        return momentum;
    }
}

