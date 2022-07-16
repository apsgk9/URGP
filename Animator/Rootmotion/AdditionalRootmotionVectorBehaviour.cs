using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdditionalRootmotionVectorBehaviour : StateMachineBehaviour
{

    public AnimationCurve VectorCurve;
    private RootMotionDeltaFixedUpdate _RootMotionDeltaFixedUpdate;
    public float Multiplier=1f;
    
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(_RootMotionDeltaFixedUpdate==null)
        {
            _RootMotionDeltaFixedUpdate=animator.GetComponent<RootMotionDeltaFixedUpdate>();
        }
        _RootMotionDeltaFixedUpdate.ForceRootMotion=true;
        
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(_RootMotionDeltaFixedUpdate==null)
        {
            _RootMotionDeltaFixedUpdate=animator.GetComponent<RootMotionDeltaFixedUpdate>();
        }
        if(_RootMotionDeltaFixedUpdate==null)
        {
            Debug.Log("NULL");
            return;
        }
        var deltaVector = VectorCurve.Evaluate(stateInfo.normalizedTime);
        _RootMotionDeltaFixedUpdate.AdditionalVectorDelta += Multiplier * deltaVector * animator.transform.forward * GetDeltaTime();

    }

    private static float GetDeltaTime()
    {
        if(Time.inFixedTimeStep)
        {
            return Time.fixedDeltaTime;
        }
        return Time.deltaTime;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(_RootMotionDeltaFixedUpdate==null)
        {
            _RootMotionDeltaFixedUpdate=animator.GetComponent<RootMotionDeltaFixedUpdate>();
        }
        if(_RootMotionDeltaFixedUpdate==null)
        {
            Debug.Log("NULL");
            return;
        }
        _RootMotionDeltaFixedUpdate.ForceRootMotion=false;        
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
