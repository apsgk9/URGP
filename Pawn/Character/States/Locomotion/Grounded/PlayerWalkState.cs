using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LocomotionEnmus;

public class PlayerWalkState : PlayerLocomotionBaseState
{
  public PlayerWalkState(Character currentContext, PlayerLocomotionStateFactory playerStateFactory)
  : base(currentContext, playerStateFactory){}

  protected override void EnterState()
  {    
  }

  public override void UpdateState(){
    EnterCheckSwitchStates();
    if(Ctx.ShouldUseRootMotion())
    {
      Ctx.ApplyRootMotion();
    }
    else
    {
      Ctx.Velocity= Ctx.WalkSpeed *Ctx.DesiredCharacterVectorForward;
    }
  }

  protected override void ExitState()
  {
  }

  public override void InitializeSubState(){}

  public override void CheckSwitchStates(){
    if (!Ctx.IsMovementPressed && Ctx.LocomotionMode==LocomotionMode.Idle) 
    {
      SwitchState(Factory.Idle());
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
