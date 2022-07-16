using System.Collections;
using System.Collections.Generic;
using Movement;
using UnityEngine;

public class NonLocomotionRisingState : NonLocomotionBaseState
{

    public NonLocomotionRisingState(CharacterAction currentContext, NonLocomotionStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory){}

    protected override void EnterState()
    {
        Ctx.AirborneMode=LocomotionEnmus.AirborneMode.Rising;
    }

    public override void UpdateState()
    {
        EnterCheckSwitchStates();
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

