using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossFadeToStateBehaviour : StateMachineBehaviour
{
    [SerializeField]
    string StateToTransitionTo;
    [SerializeField] [Range(0,1)]
    float ExitTime = 0.25f;
    [SerializeField]
    bool UseFixedTime = false;
    [SerializeField] [Min(0)]
    float TimeDuration = 0.25f;
    [SerializeField]
    bool transitioned = false;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        transitioned=false;
                
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(transitioned==false)
        {
            if(stateInfo.normalizedTime>ExitTime)
            {
                if(UseFixedTime)
                {
                    animator.CrossFadeInFixedTime(StateToTransitionTo,TimeDuration);
                }
                else
                {
                    animator.CrossFade(StateToTransitionTo,TimeDuration);
                }
                transitioned=true;
            }
        }        
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{    
    //}

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
