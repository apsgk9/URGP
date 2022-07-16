using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

namespace AnimatorItem
{
    public class AnimatorTransitionItem : ScriptableObject
    {
        public AnimatorStateItem From;
        public AnimatorStateItem To;
        public AnimatorConditionItem Condition;        
    }
}