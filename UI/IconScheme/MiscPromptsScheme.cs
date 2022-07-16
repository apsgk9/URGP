using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GamePrompts
{
    [CreateAssetMenu(fileName = "MiscPromptsScheme", menuName = "ScriptableObjects/PromptScheme/Misc", order = 100)]
    public class MiscPromptsScheme : ScriptableObject
    {

        [Header("Miscellaneous")]
        public Sprite Unknown;
    }
}