using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnimatorItem
{
    public class AnimatorComboStateMachineItem : ScriptableObject
    {
        public AnimatorTransitionItem[] AnimatorTransitionItems;
        public AnimatorStateItem[] AnimatorStateItems;
    }
}