using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AnimatorStateMachineEnums;

public partial class SetBooleanParamaterListBehaviour : StateMachineBehaviour
{
    public BooleanParameter[] BooleanList;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        foreach(BooleanParameter BooleanParameter in BooleanList)
        {
            if(BooleanParameter.SetAt==SetAt.Enter)
            {
                animator.SetBool(BooleanParameter.ParameterName,BooleanParameter.SetTo);
            }
        }
        //if(Set==SetAt.Enter)
        //{
        //    animator.SetBool(ParameterName,SetTo);
        //}
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        foreach(BooleanParameter BooleanParameter in BooleanList)
        {
            if(BooleanParameter.SetAt==SetAt.Exit)
            {
                animator.SetBool(BooleanParameter.ParameterName,BooleanParameter.SetTo);
            }
        }        
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
