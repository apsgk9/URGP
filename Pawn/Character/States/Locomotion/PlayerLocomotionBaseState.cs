using UnityEngine;

//Doesn't Run Enterstate of initialized substate 
//The current substate that is initialized by a Super state isActive.
public abstract class PlayerLocomotionBaseState : BaseState<PlayerLocomotionBaseState>
{
    private Character _ctx;
    private PlayerLocomotionStateFactory _factory;
    protected Character Ctx { get { return _ctx; } }
    protected PlayerLocomotionStateFactory Factory { get { return _factory; } }

    public PlayerLocomotionBaseState(Character currentContext, PlayerLocomotionStateFactory playerStateFactory)
    {
        _ctx = currentContext;
        _factory = playerStateFactory;
    }

    protected override void SetCurrentState(IBaseState newState)
    {
        _ctx.CurrentLocomotionState=newState as PlayerLocomotionBaseState;
    }

    protected override bool CanTransition()
    {
        return _ctx._LocomotionStateMachineCanTransition;
    }


}
