public interface IBaseState
{
    void CheckSwitchStates();
    void Enter();
    void Exit();
    void InitializeSubState();
    void UpdateState();
    void UpdateStates();
}
