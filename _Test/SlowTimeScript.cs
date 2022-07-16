using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SlowTimeScript : MonoBehaviour
{
    public float SlowDownTime = 0.25f;
    
    [Range(0.05f,5f)]
    public float DurationToChangeTime = 1f;
    public float CurrentTime;
    public float TimeScaleToSet;
    public float PreviousTimeScaleToSet;

    public float FixedTimeScaleToSet ;
    public float PreviousFixedTimeScaleToSet;

    public bool _slowDownTime=false;
    
    public bool _ChangeTime=false;

    private void Awake()
    {
        CurrentTime=Time.unscaledTime;
    }
    private void Start()
    {        
        SetToNormal();
    }

    

    private void Update()
    {
        if (_ChangeTime==false)
            return;
        float val=GetRatioNormalTime();
        float toSet = Mathf.Lerp(PreviousTimeScaleToSet, TimeScaleToSet,val );
        float toSetFixed = Mathf.Lerp(PreviousFixedTimeScaleToSet, FixedTimeScaleToSet,val);
        Time.timeScale = toSet;
        Time.fixedDeltaTime = toSetFixed;
        if (val >= 1)
        {
            PreviousTimeScaleToSet = TimeScaleToSet;
            PreviousFixedTimeScaleToSet = FixedTimeScaleToSet;
            _ChangeTime=false;
        }
    }

    private float GetRatioNormalTime()
    {
        return Mathf.Min(1, (Time.unscaledTime - CurrentTime) / DurationToChangeTime);
    }
    private void OnEnable()
    {
        UserInput.Instance.PlayerInputActions.PlayerControls.TimeButton.started += TimeChange;        
    }
    private void OnDisable()
    {
        if(UserInput.CanAccess)
            UserInput.Instance.PlayerInputActions.PlayerControls.TimeButton.started -= TimeChange;        
    }

    private void TimeChange(InputAction.CallbackContext obj)
    {
        if(_slowDownTime==false)
        {
            Slowdown();
        }
        else
        {
            ReturnToNormal();
        }
    }

    private void ReturnToNormal()
    {
        Debug.LogWarning("START NORMAL");
        _slowDownTime = false;
        SetToNormal();

        //CameraManager.Instance.PlayerCameraManagerReference.SetIgnoreTime(false);

        _ChangeTime = true;

        CurrentTime= Time.unscaledTime;
    }

    private void SetToNormal()
    {
        TimeScaleToSet = 1.0f;
        PreviousTimeScaleToSet = SlowDownTime;


        FixedTimeScaleToSet = 1 / GameState.GetTickRate();
        PreviousFixedTimeScaleToSet = 1 / GameState.GetTickRate() * SlowDownTime;
    }

    private void Slowdown()
    {
        Debug.LogWarning("START SLOWDOWN");
        _slowDownTime = true;

        
        TimeScaleToSet = SlowDownTime;
        PreviousTimeScaleToSet=1.0f;

        FixedTimeScaleToSet = 1/GameState.GetTickRate() * SlowDownTime;
        PreviousFixedTimeScaleToSet=1/GameState.GetTickRate();

        //CameraManager.Instance.PlayerCameraManagerReference.SetIgnoreTime(true);
        _ChangeTime=true;
        CurrentTime= Time.unscaledTime;
    }

}
