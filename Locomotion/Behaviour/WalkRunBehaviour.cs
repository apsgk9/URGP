using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkRunBehaviour : StateMachineBehaviour
{
    public string SpeedParameterName="Speed";
    public string LastSpeedParameterName="LastSpeed";
    public string WalkRunSpeedParameterName="WalkRunSpeed";
    public string FirstSpeedParameterName="FirstSpeedValue";
    private bool _isTransitioning;
    private float initialOffset;
    private float currentTime;

    public float SpeedValue { get; private set; }
    public float LastSpeedValue { get; private set; }
    public float WalkRunSpeedValue { get; private set; }
    public float PreviousWalkRunSpeedValue { get; private set; }
    
    public float RunToWalkTransitionTime=0.25f;
    public float WalkToRunTransitionTime=0.15f;
    private float TransitionTimeToUse;
    public AnimationCurve AnimationCurveToUse { get; private set; }
    public AnimationCurve RunToWalkTransitionCurve= AnimationCurve.Linear(0,0,1,1);
    public AnimationCurve WalkToRunTransitionCurve= AnimationCurve.Linear(0,0,1,1);

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SpeedValue= animator.GetFloat(SpeedParameterName);
        animator.SetFloat(FirstSpeedParameterName,SpeedValue);
        LastSpeedValue= animator.GetFloat(LastSpeedParameterName);
        PreviousWalkRunSpeedValue =animator.GetFloat(WalkRunSpeedParameterName);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
        SpeedValue= animator.GetFloat(SpeedParameterName);
        LastSpeedValue= animator.GetFloat(LastSpeedParameterName);
        PreviousWalkRunSpeedValue= animator.GetFloat(WalkRunSpeedParameterName);

        WalkRunSpeedValue = GetWalkRunSpeedValue();
        animator.SetFloat(WalkRunSpeedParameterName,WalkRunSpeedValue);
        if(SpeedValue!=0f)
        {
            animator.SetFloat(LastSpeedParameterName,SpeedValue);
        }
    }


    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
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

    
    private float GetWalkRunSpeedValue()
    {
        if(_isTransitioning)
        {
            var canStillTransition= CheckifTransitionIsStillValid(TransitionTimeToUse);
            if(canStillTransition)
            {
                return Transition(TransitionTimeToUse,AnimationCurveToUse);
            }
            else
            {
                if (currentTime >= TransitionTimeToUse && _isTransitioning==true)
                {
                    _isTransitioning=false;
                    return SpeedValue;
                }                
                _isTransitioning=false;                
            }
        }
        else
        {
            bool shouldTransition= CheckTransition();
            if(shouldTransition)
            {
                SetupTransition();
                return Transition(TransitionTimeToUse,AnimationCurveToUse);
            }
        }

        return SpeedValue;
    }

    private void SetupTransition()
    {
        _isTransitioning=true;
        initialOffset=PreviousWalkRunSpeedValue;
        currentTime = 0f;
        TransitionTimeToUse = GetTransitionTimeToUse();
        AnimationCurveToUse = GetAnimationCurveToUse();
    }

    private AnimationCurve GetAnimationCurveToUse()
    {
        if(SpeedValue==1)
        {
            return RunToWalkTransitionCurve;            
        }
        return WalkToRunTransitionCurve;
    }

    private float GetTransitionTimeToUse()
    {
        if(SpeedValue==1)
        {
            return RunToWalkTransitionTime;            
        }
        return WalkToRunTransitionTime;                
    }


    private bool CheckTransition()
    {
        if(SpeedValue<0.01f)
        {
            return false;
        }

        if(PreviousWalkRunSpeedValue!=SpeedValue)
        {
            return true;
        }
        return false;

    }

    private float Transition(float TransitionTime, AnimationCurve AnimationCurve)
    {
        var newTime = currentTime / TransitionTime;
        var BlendedValue = initialOffset + AnimationCurve.Evaluate(newTime) * (SpeedValue - initialOffset);
        currentTime += Time.deltaTime;
        return BlendedValue;
    }

    private bool CheckifTransitionIsStillValid(float TransitionTime)
    {
        return currentTime <= TransitionTime && _isTransitioning == true;
    }
}
