using System.Collections;
using System.Collections.Generic;
using Movement;
using UnityEngine;

//Kinda not named well but just like a modifier to the super state so it doesn't get clogged
public class PlayerFallingState : PlayerLocomotionBaseState
{

    public PlayerFallingState(Character currentContext, PlayerLocomotionStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory){}

    protected override void EnterState()
    {
        Ctx.AirborneMode=LocomotionEnmus.AirborneMode.Falling;
    }

    public override void UpdateState()
    {
        EnterCheckSwitchStates();
        Ctx.SavedMomentum+=-Vector3.up*Ctx.AdditionalFallingSpeed;
    }



    protected override void ExitState()
    {
    }

    public override void InitializeSubState()
    {
    }

    public override void CheckSwitchStates()
    {  
        if(Ctx.IsJumping)
        {
            SwitchState(Factory.Jump());
        }
        else if(Ctx.Mover.GetVelocity().y>0)
        {            
            SwitchState(Factory.Rising());
        }
    }
}

