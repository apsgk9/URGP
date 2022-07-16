using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LocomotionEnmus;

public class PlayerIdleState : PlayerLocomotionBaseState
{
  public PlayerIdleState(Character currentContext, PlayerLocomotionStateFactory playerStateFactory)
  : base(currentContext, playerStateFactory){}

  protected override void EnterState()
  {

  }

  public override void UpdateState()
  {
    EnterCheckSwitchStates();
    if(Ctx.ShouldUseRootMotion())
    {
      Ctx.ApplyRootMotion();
    }
    else
    {
      Ctx.Velocity= Vector3.zero;
    }
  }

  protected override void ExitState()
  {
  }

  public override void InitializeSubState(){}

  public override void CheckSwitchStates()
  {
    if (Ctx.IsMovementPressed && Ctx.LocomotionMode==LocomotionMode.Walk) 
    {
      SwitchState(Factory.Walk());
    }
    else if (Ctx.IsMovementPressed && Ctx.LocomotionMode==LocomotionMode.Run) 
    {
      SwitchState(Factory.Run());
    }
    else if (Ctx.IsMovementPressed && Ctx.LocomotionMode==LocomotionMode.Sprint) 
    {
      SwitchState(Factory.Sprint());
    }
  }
}
