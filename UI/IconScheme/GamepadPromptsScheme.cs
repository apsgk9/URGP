using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GamePrompts
{
    [CreateAssetMenu(fileName = "GamepadPromptsScheme", menuName = "ScriptableObjects/PromptScheme/Gamepad", order = 1)]
    public class GamepadPromptsScheme : ScriptableObject
    {
        [Header("Buttons")]
        public Sprite ButtonNorth;
        public Sprite ButtonSouth;
        public Sprite ButtonEast;
        public Sprite ButtonWest;

        [Header("D-Pad")]
        public Sprite DPadNorth;
        public Sprite DPadSouth;
        public Sprite DPadEast;
        public Sprite DPadWest;

        [Header("Stick")]
        public Sprite LeftStick;
        public Sprite LeftStickPress;
        public Sprite RightStick;
        public Sprite RightStickPress;

        [Header("Shoulder Buttons")]
        public Sprite LeftShoulder;
        public Sprite LeftTrigger;
        public Sprite RightShoulder;
        public Sprite RightTrigger;
    }

}