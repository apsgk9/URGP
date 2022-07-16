using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace IAUtil
{
    public static class InputActionsUtility
    {
        public static string GetReadableTextFromInputActionReference(InputActionReference ActionCommand)
        {
            int bindingIndex= ActionCommand.action.GetBindingIndexForControl(ActionCommand.action.controls[0]);
            return InputControlPath.ToHumanReadableString(ActionCommand.action.bindings[bindingIndex].effectivePath,InputControlPath.HumanReadableStringOptions.OmitDevice);
        }
    }
}