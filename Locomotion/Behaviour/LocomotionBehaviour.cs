using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocomotionBehaviour : StateMachineBehaviour
{
    public string SpeedParameterName="Speed";
    public string SpeedMultiplierParameterName="LocomotionSpeedMultiplier";
    public float WalkSpeedThreshold=1f;
    public float RunSpeedThreshold=2f;
    public float SprintSpeedThreshold=3f;
    public AnimatorStateInfo _stateInfo;
    public AnimationCurve WalkSpeedCurve;
    public AnimationCurve RunSpeedCurve;
    public AnimationCurve SprintSpeedCurve;
    public float SpeedValue;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _stateInfo=stateInfo;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {        
        SpeedValue= animator.GetFloat(SpeedParameterName);
        if(SpeedValue<=WalkSpeedThreshold)
        {
            float newValue = WalkSpeedCurve.Evaluate(SpeedValue/WalkSpeedThreshold);
            animator.SetFloat(SpeedMultiplierParameterName,newValue);

        }
        else if(SpeedValue<=RunSpeedThreshold)
        {
            float newValue = RunSpeedCurve.Evaluate(SpeedValue/RunSpeedThreshold);
            animator.SetFloat(SpeedMultiplierParameterName,newValue);

        }
        else if(SpeedValue<=SprintSpeedThreshold)
        {
            float newValue = SprintSpeedCurve.Evaluate(SpeedValue/SprintSpeedThreshold);
            animator.SetFloat(SpeedMultiplierParameterName,newValue);

        }
        else if(SpeedValue>SprintSpeedThreshold)
        {
            animator.SetFloat(SpeedMultiplierParameterName,SpeedValue-RunSpeedThreshold);
        }
        else
        {
            animator.SetFloat(SpeedMultiplierParameterName,1f);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }
    private void Reset()
    {
        SpeedParameterName="Speed";
        SpeedMultiplierParameterName="LocomotionSpeedMultiplier";
        WalkSpeedThreshold=1f;
        RunSpeedThreshold=2f;
        SprintSpeedThreshold=3f;  
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
