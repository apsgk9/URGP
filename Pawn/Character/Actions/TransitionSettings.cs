using System;
using UnityEditor;
using UnityEngine;
using static LocomotionEnmus;

[Serializable]
public class TransitionSettings
{
    [SerializeField] [Min(0)]
    [Tooltip("Exit time in normalized time")]
    public float ExitTime = 0.25f;
    [SerializeField]
    public bool FixedDuration = false;
    [SerializeField] [Min(0)]
    public float TimeDuration = 0.25f;
    [SerializeField] [Min(0)]
    public float TransitionOffset = 0f;

    public string StateToTransitionTo { get; internal set; }
}

[Serializable]
public class AnimatorLocomotionTransition
{
    [SerializeField]
    public string StateToTransitionToAtEnd;
    [SerializeField] [Min (0)]
    public int Layer=0;

    public bool ChangeStart=false;
    [SerializeField]
    public LocomotionModeSwitch StartLocomotionModeSwitch;
    public bool ChangeEnd=false;

    [SerializeField]
    public LocomotionModeSwitch EndLocomotionModeSwitch;
}
[Serializable]
public class AnimatorGroundedLocomotionTransition
{
    [SerializeField]
    public string StateToTransitionToAtEnd;
    [SerializeField] [Min (0)]
    public int Layer=0;

    public bool ChangeStart=false;
    [SerializeField]
    public LocomotionMode  StartLocomotionMode=LocomotionMode.Idle;
    public bool ChangeEnd=false;

    [SerializeField]
    public LocomotionMode  EndLocomotionMode=LocomotionMode.Idle;
}


[Serializable]
public class LocomotionModeSwitch
{
    
    [SerializeField]
    public bool Toggle= true;

    [SerializeField]
    public LocomotionMode  LocomotionMode=LocomotionMode.Idle;
    [SerializeField]
    public AirborneMode  AirborneMode=AirborneMode.Falling;

}
