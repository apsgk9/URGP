using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState<T>: IBaseState where T : BaseState<T>
{


    //State can choose whether or not to update if it's just been activated
    private bool _isActive = false;
    protected bool IsActive { get { return _isActive; } }

    protected bool _isRootState = false;
    protected T _currentSubState;
    protected T _currentSuperState;

    protected bool IsRootState { set { _isRootState = value; } }
    virtual public void Enter()
    {
        _isActive = true;
        LogState("Entered: " + this.GetType().Name);
        EnterState();
        if (_currentSubState != null)
        {
            _currentSubState.Enter();
        }
    }

    protected abstract void EnterState();
    public abstract void UpdateState();
    virtual public void Exit()
    {
        _isActive = false;
        LogState("Exited: " + this.GetType().Name);
        ExitState();
        if (_currentSubState != null)
        {
            _currentSubState.Exit();
        }
    }
    protected abstract void ExitState();
    virtual protected void EnterCheckSwitchStates()
    {
        if (CanTransition())
        {
            CheckSwitchStates();
        }
    }
    public abstract void CheckSwitchStates();
    public abstract void InitializeSubState();

    virtual public void UpdateStates()
    {
        UpdateState();
        if (_currentSubState != null && _isActive)
        {
            _currentSubState.UpdateStates();
        }
    }

    virtual protected void SwitchState(T newState)
    {
        // current state exits state
        Exit();

        // new state enters state
        newState.Enter();

        if (_isRootState)
        {
            // switch current state of context
            SetCurrentState(newState);
        }
        else if (_currentSuperState != null)
        {
            // set the current super states sub state to the new state
            _currentSuperState.SetSubState(newState);
        }

    }

    virtual protected void SetSuperState(T newSuperState)
    {
        _currentSuperState = newSuperState;
    }

    virtual protected void SetSubState(T newSubState)
    {
        newSubState._isActive = true;
        _currentSubState = newSubState;
        newSubState.SetSuperState(this as T);
    }

    virtual protected void LogState(string log)
    {
        //Debug.Log(log);
    }
    
    protected abstract void SetCurrentState(IBaseState newState);
    protected abstract bool CanTransition();
}
