using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PartySystem
{
    public static class CharacterSwitcher
    {
        /*
            Takes Control of new unit and assigns a virtual player camera to the new unit.
        */
        public static bool SwitchUnit(GameObject OldUnit, GameObject NewUnit)
        {
            if (NewUnit == null)
            {
                Debug.LogError("Unable to assign to a new unit that doesn't exist.");
                return false;
            }
            ////ASSIGN NEW CAMERA FIRST THEN NEW CHARACTER, or else it breaks
            //bool success = AssignNewPlayerCamera();
            //if (!success)
            //{
            //    Debug.LogError("Unable to assign new camera to switch characters");
            //    return false;
            //}
            SwitchControlToNewMainUnit(OldUnit, NewUnit);
            GameMode.IPlayerCentric.SwitchCameraToNewPlayer?.Invoke(OldUnit,NewUnit);
            return true;
        }

        private static void SwitchControlToNewMainUnit(GameObject OldUnit, GameObject NewUnit)
        {
            OldUnit.GetComponent<UnitSystem.GameUnit>().PlayerRemoveControl();
            NewUnit.GetComponent<UnitSystem.GameUnit>().PlayerTakeControl();
            GameMode.IPlayerCentric.Player = NewUnit;
        }

        private static bool AssignNewPlayerCamera()
        {
            var newFreePlayerCamera = CameraManager.Instance.GetNextFreelookPlayerCameraToChangedTo();
            if (newFreePlayerCamera == null)
            {
                Debug.LogError("newFreePlayerCamera == null");
                return false;
            }

            bool success = CameraManager.Instance.SetMainCameraPlayer(newFreePlayerCamera);
            if (success == false)
            {
                Debug.LogError("Unable to  set Main Camera Player.");
                return false;
            }

            return true;
        }

    }
}

