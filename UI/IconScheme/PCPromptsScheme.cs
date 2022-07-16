using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GamePrompts
{
    [CreateAssetMenu(fileName = "KeyPromptsScheme", menuName = "ScriptableObjects/PromptScheme/PC", order = 0)]
    public class PCPromptsScheme : ScriptableObject
    {

        [Header("Mouse")]
        public Sprite Mouse;
        public Sprite MouseLeft;
        public Sprite MouseRight;
        public Sprite MouseMiddle;
    }
}