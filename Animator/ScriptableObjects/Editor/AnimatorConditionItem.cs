using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

namespace AnimatorItem
{
    public class AnimatorConditionItem : ScriptableObject
    {
        public AnimatorConditionMode AnimatorConditionMode;
        public float Threshold;
        public string Parameter;
    }
}
