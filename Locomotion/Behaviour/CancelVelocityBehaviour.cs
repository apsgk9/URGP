using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AnimatorStateMachineEnums;

public class CancelVelocityBehaviour : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public SetAt SetAt;
    IMover CharacterMover;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(CharacterMover==null)
        {
            CharacterMover=animator.GetComponentInParent<IMover>();
        }
        if(SetAt.Enter==SetAt)
        {            
            CharacterMover.SetVelocity(Vector3.zero);
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {        
        if(SetAt.Exit==SetAt)
        {            
            CharacterMover.SetVelocity(Vector3.zero);
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
