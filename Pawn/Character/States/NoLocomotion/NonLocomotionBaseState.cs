using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NonLocomotionBaseState : BaseState<NonLocomotionBaseState>
{
    private CharacterAction _ctx;
    private NonLocomotionStateFactory _factory;
    protected CharacterAction Ctx { get { return _ctx; } }
    protected NonLocomotionStateFactory Factory { get { return _factory; } }

    public NonLocomotionBaseState(CharacterAction currentContext, NonLocomotionStateFactory playerStateFactory)
    {
        _ctx = currentContext;
        _factory = playerStateFactory;
    }

    protected override void SetCurrentState(IBaseState newState)
    {
        _ctx.CurrentNonLocomotionState=newState as NonLocomotionBaseState;
    }

    protected override bool CanTransition()
    {
        return _ctx._NonLocomotionStateMachineCanTransition;
    }

}
