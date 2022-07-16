using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegCycleBehaviour : StateMachineBehaviour
{
    [SerializeField] [Range(0f,1f)] float RightFootDownStart=0.5f;
    [SerializeField] [Range(0f,1f)] float LeftFootDownStart=0f;
    //[SerializeField] [Range(0f,0.1f)] float gap=0.025f;
    [SerializeField] string RightFootDownParameterName="RightFootDown";
    [SerializeField] string LeftFootDownParameterName="LeftFootDown";
    [SerializeField] bool UseFloat=false;
    private float stateTime;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //problem if loops around 1 or 0, TODO: Fix this
        stateTime=stateInfo.normalizedTime;
        if(stateTime>1f)
        {
            stateTime=stateTime%1;
        }
        if(LeftFootDownStart<stateTime && stateTime<RightFootDownStart)
        {
            if(UseFloat)
            {
                animator.SetFloat(RightFootDownParameterName,1.0f);
                animator.SetFloat(LeftFootDownParameterName,0.0f);

            }
            else
            {
                animator.SetBool(RightFootDownParameterName,true);
                animator.SetBool(LeftFootDownParameterName,false);
            }
        }
        else
        {
            if(UseFloat)
            {
                
                animator.SetFloat(RightFootDownParameterName,0.0f);
                animator.SetFloat(LeftFootDownParameterName,1.0f);
            }
            else
            {
                animator.SetBool(RightFootDownParameterName,false);
                animator.SetBool(LeftFootDownParameterName,true);
            }
        }
        //if(LeftFootDownStart-gap<stateInfo.normalizedTime && stateInfo.normalizedTime<LeftFootDownStart+gap)
        //{
        //    animator.SetBool(RightFootDownParameterName,false);
        //    animator.SetBool(LeftFootDownParameterName,true);
        //}
        
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }

    //// OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    //// OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
