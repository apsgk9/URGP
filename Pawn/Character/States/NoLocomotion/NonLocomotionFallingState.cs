using System.Collections;
using System.Collections.Generic;
using Movement;
using UnityEngine;

public class NonLocomotionFallingState : NonLocomotionBaseState
{

    public NonLocomotionFallingState(CharacterAction currentContext, NonLocomotionStateFactory playerStateFactory)
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
        if(Ctx.Mover.GetVelocity().y>0)
        {            
            SwitchState(Factory.Falling());
        }
    }
}

