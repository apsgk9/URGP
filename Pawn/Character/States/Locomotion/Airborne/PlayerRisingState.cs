using System.Collections;
using System.Collections.Generic;
using Movement;
using UnityEngine;

//Kinda not named well but just like a modifier to the super state so it doesn't get clogged
public class PlayerRisingState : PlayerLocomotionBaseState
{

    public PlayerRisingState(Character currentContext, PlayerLocomotionStateFactory playerStateFactory)
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
        if(Ctx.CeilingDetector!=null && Ctx.CeilingDetector.HitCeiling())
        {     
			Ctx.SavedMomentum = VectorMath.RemoveDotVector(Ctx.SavedMomentum, Ctx.CharacterGameObject.transform.up);
            SwitchState(Factory.Falling());
            return;
        }

        if(Ctx.Mover.GetVelocity().y<0)
        {            
            SwitchState(Factory.Falling());
        }
        
    }
}

