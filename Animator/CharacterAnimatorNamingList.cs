using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimatorNamingList : ScriptableObject
{
    [Header("ParameterList")]
    public string SpeedParameterName = "Speed";
    public int SpeedHash {get{return Animator.StringToHash(SpeedParameterName);}} 
    public string LocomotionModeParameterName = "LocomotionMode";
    public int LocomotionModeHash {get{return Animator.StringToHash(LocomotionModeParameterName);}} 
    public string AirborneModeParameterName = "AirborneMode";
    public int AirborneModeHash {get{return Animator.StringToHash(AirborneModeParameterName);}} 
    public string MovementPressedParameterName ="MovementPressed";
    public int MovementPressedHash {get{return Animator.StringToHash(MovementPressedParameterName);}} 
    public string UsingControllerParameterName ="UsingController";
    public int UsingControllerHash {get{return Animator.StringToHash(UsingControllerParameterName);}} 
    public string ControllerDeltaParameterName ="ControllerDelta";
    public int ControllerDeltaHash {get{return Animator.StringToHash(ControllerDeltaParameterName);}} 
    public string CharacterHasStaminaParameterName ="HasStamina";
    public int CharacterHasStaminaHash {get{return Animator.StringToHash(CharacterHasStaminaParameterName);}} 
    public string JumpTriggerParameterName ="Jump";
    public int JumpTriggerHash {get{return Animator.StringToHash(JumpTriggerParameterName);}} 
    public string IsJumpingParameterName ="IsJumping";
    public int IsJumpingHash {get{return Animator.StringToHash(IsJumpingParameterName);}} 
    public string isGroundedParameterName ="isGrounded";
    public int isGroundedHash {get{return Animator.StringToHash(isGroundedParameterName);}} 
    public string InterruptableParameterName ="Interruptable";
    public int InterruptableHash {get{return Animator.StringToHash(InterruptableParameterName);}} 
    public string CanRotateParameterName = "CanRotate";
    public int CanRotateHash {get{return Animator.StringToHash(CanRotateParameterName);}} 
    public string NormalizedTimeParameterName = "NormalizedTime";
    public int NormalizedTimeHash {get{return Animator.StringToHash(NormalizedTimeParameterName);}} 

    [Header("StateList")]
    public string IdleStateName = "Idle";
    public string RunStateName = "Run";
    public string WalkStateName = "Walk";
    public string StartRunStateName = "StartRun";
    public string StopLeftStateName = "StopLeft";
    public string StopRightStateName = "StopRight";
}
