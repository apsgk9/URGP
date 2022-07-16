using UnityEngine;

namespace InputHelper
{
    public class DeviceInputTool
    {
        public static bool IsUsingController()
        {
            var deviceUsing = UserInput.Instance.DeviceUsing;
            if (UsesControllerInput(deviceUsing))
            {
                return true;
            }
            return false;
        }

        private static bool UsesControllerInput(string deviceUsing)
        {
            switch(deviceUsing)
            {
                case "DualShock4GamepadHID": 
                case "XInputControllerWindows":
                    return true;
                default:
                    return false;
            }
        }
    }
}