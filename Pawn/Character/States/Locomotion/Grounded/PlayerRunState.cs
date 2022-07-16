using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LocomotionEnmus;

public class PlayerRunState : PlayerLocomotionBaseState
{
  public PlayerRunState(Character currentContext, PlayerLocomotionStateFactory playerStateFactory)
  : base(currentContext, playerStateFactory) {}

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
      float DdotF=Vector3.Dot(Ctx.DesiredCharacterVectorForward,Ctx.CharacterGameObject.transform.forward);
      float multipler=(DdotF+1)/2;
      Ctx.Velocity= multipler*Ctx.RunSpeed*Ctx.DesiredCharacterVectorForward;
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
    else if (Ctx.IsMovementPressed && Ctx.LocomotionMode==LocomotionMode.Walk) 
    {
      SwitchState(Factory.Walk());
    }
    else if (Ctx.IsMovementPressed && Ctx.LocomotionMode==LocomotionMode.Sprint) 
    {
      SwitchState(Factory.Sprint());
    }
  }
}
