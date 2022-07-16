using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public static class AnimatorUtil
{
    public static List<string> GetParameters(AnimatorController animatorController,out List<AnimatorControllerParameterType> Types)
    {
        List<string> ParameterList= new List<string>();
        Types= new List<AnimatorControllerParameterType>();
        if (animatorController != null)
        {
            
            var Paramaters = animatorController.parameters;
            foreach(var p in Paramaters)
            {
                ParameterList.Add(p.name);
                Types.Add(p.type);
            }
        }
        return ParameterList;
    }
    public static List<string> GetListOfStates(AnimatorController animatorController, int Layer)
    {
        List<string> ListsOfStates= new List<string>();
        if (animatorController != null && 0<=Layer && Layer<animatorController.layers.Length)
        {
            var rootStateMachine = animatorController.layers[Layer].stateMachine;
            foreach (var childAnimatorstate in rootStateMachine.states)
            {
                ListsOfStates.Add(childAnimatorstate.state.name);                
            }
            foreach (var childAnimatorStateMachine in rootStateMachine.stateMachines)
            {
                ListsOfStates.AddRange(GetListOfStatesFromStateMachine(childAnimatorStateMachine));                
            }
        }
        return ListsOfStates;
    }
    public static List<string> GetListOfStates(AnimatorController animatorController)
    {
        List<string> ListsOfStates= new List<string>();
        if (animatorController != null)
        {
            for (int i = 0; i < animatorController.layers.Length; i++)
            {
                var rootStateMachine = animatorController.layers[i].stateMachine;
                foreach (var childAnimatorStateMachine in rootStateMachine.stateMachines)
                {
                    ListsOfStates.AddRange(GetListOfStatesFromStateMachine(childAnimatorStateMachine));
                    
                }
            }
        }
        return ListsOfStates;
    }
    public static List<string> GetListOfStatesFromStateMachine(ChildAnimatorStateMachine ChildAnimatorStateMachine)
    {
        List<string> ListsOfStates= new List<string>();
        var AnimatorStateMachine = ChildAnimatorStateMachine.stateMachine;
        if (isAnimatorStateMachineEmpty(AnimatorStateMachine))
            return ListsOfStates;


        //Search Each State
        foreach (var childAnimatorState in AnimatorStateMachine.states)
        {
            ListsOfStates.Add(childAnimatorState.state.name);
        }

        foreach (var childAnimatorStateMachine in AnimatorStateMachine.stateMachines)
        {            
            ListsOfStates.AddRange(GetListOfStatesFromStateMachine(childAnimatorStateMachine));
        }
        return ListsOfStates;
    }

    //-------------DoesStateExist
    public static bool DoesStateExist(AnimatorController animatorController, string stateName)
    {
        if (animatorController != null)
        {
            for (int i = 0; i < animatorController.layers.Length; i++)
            {
                var rootStateMachine = animatorController.layers[i].stateMachine;
                foreach (var childAnimatorStateMachine in rootStateMachine.stateMachines)
                {
                    if (FindStateWithName(childAnimatorStateMachine, stateName))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    public static bool FindStateWithName(ChildAnimatorStateMachine ChildAnimatorStateMachine, string stateNameTofind)
    {
        var AnimatorStateMachine = ChildAnimatorStateMachine.stateMachine;
        if (isAnimatorStateMachineEmpty(AnimatorStateMachine))
            return false;


        //Search Each State
        foreach (var childAnimatorState in AnimatorStateMachine.states)
        {
            if (childAnimatorState.state.name == stateNameTofind)
            {
                return true;
            }
        }

        foreach (var childAnimatorStateMachine in AnimatorStateMachine.stateMachines)
        {
            if (FindStateWithName(childAnimatorStateMachine, stateNameTofind))
            {
                return true;
            }
        }
        return false;
    }

    //-----------------------------------


    //-------------DoesStateExist

    public static UnityEditor.Animations.AnimatorState GetStateWithName(AnimatorController animatorController, string stateName)
    {
        if (animatorController != null)
        {
            for (int i = 0; i < animatorController.layers.Length; i++)
            {
                var rootStateMachine = animatorController.layers[i].stateMachine;
                foreach (var childAnimatorStateMachine in rootStateMachine.stateMachines)
                {
                    var animState =GetStateWithName(childAnimatorStateMachine, stateName);
                    if (animState)
                    {
                        return animState;
                    }
                }
            }
        }
        return null;
    }
    public static UnityEditor.Animations.AnimatorState GetStateWithName(ChildAnimatorStateMachine ChildAnimatorStateMachine, string stateNameTofind)
    {
        var AnimatorStateMachine = ChildAnimatorStateMachine.stateMachine;
        if (isAnimatorStateMachineEmpty(AnimatorStateMachine))
            return null;


        //Search Each State
        foreach (var childAnimatorState in AnimatorStateMachine.states)
        {
            if (childAnimatorState.state.name == stateNameTofind)
            {
                return childAnimatorState.state;
            }
        }

        foreach (var childAnimatorStateMachine in AnimatorStateMachine.stateMachines)
        {
            var animState = GetStateWithName(childAnimatorStateMachine, stateNameTofind);
            if (animState)
            {
                return animState;
            }
        }
        return null;
    }

    private static bool isAnimatorStateMachineEmpty(AnimatorStateMachine AnimatorStateMachine)
    {
        return AnimatorStateMachine.states.Length <= 0 && AnimatorStateMachine.stateMachines.Length <= 0;
    }


    //-------------GetAnimatorStatesInfo
    //Debug
    public static string GetAnimatorStatesInfo(AnimatorController animatorController)
    {


        if (animatorController != null)
        {

            string animatorInfoText = "<b>Animator:</b>" + animatorController.name + "\n";

            for (int i = 0; i < animatorController.layers.Length; i++)
            {
                var rootStateMachine = animatorController.layers[i].stateMachine;
                animatorInfoText += "\n<b>Layer:</b>" + animatorController.layers[i].name;
                animatorInfoText += "\n<b>Length:</b>" + rootStateMachine.stateMachines.Length;
                foreach (var childAnimatorStateMachine in rootStateMachine.stateMachines)
                {
                    animatorInfoText += GetAnimatorStatesInfo(childAnimatorStateMachine, 1);
                }
            }


            return animatorInfoText;
        }
        return "No Info Found..";
    }
    public static string GetAnimatorStatesInfo(ChildAnimatorStateMachine ChildAnimatorStateMachine, int numTabs)
    {
        string animatorInfoText = "";
        string extraTab = "";
        for (int i = 0; i < numTabs; i++)
        {
            extraTab += "\t";
        }
        var AnimatorStateMachine = ChildAnimatorStateMachine.stateMachine;
        animatorInfoText += "\n" + extraTab + "-----------------------" + AnimatorStateMachine.name + "-----------------------";
        if (isAnimatorStateMachineEmpty(AnimatorStateMachine))
            return "";


        //do states
        foreach (var childAnimatorState in AnimatorStateMachine.states)
        {
            animatorInfoText += "\n" + extraTab + "\t-----------------------";
            animatorInfoText += "\n" + extraTab + "\t<b>State:</b>" + childAnimatorState.state.name;
            animatorInfoText += "\n" + extraTab + "\t<b>State Name Hash:</b>" + childAnimatorState.state.nameHash;
        }
        foreach (var childAnimatorStateMachine in AnimatorStateMachine.stateMachines)
        {
            animatorInfoText += extraTab + GetAnimatorStatesInfo(childAnimatorStateMachine, numTabs + 1);
        }
        animatorInfoText += "\n";

        return animatorInfoText;
    }
}
