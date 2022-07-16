using System;
using System.Collections;
using System.Collections.Generic;
using Controller;
using UnitSystem;
using Service;
using UnityEngine;

namespace GameMode
{
    public abstract class PlayerCentricMode<T> : GameModeBase<PlayerCentricMode<T>,T>,IPlayerCentric where T: Component
    {
    
        public static Transform GetCameraTargetOnPlayer()
        {
            
            var Target = IPlayerCentric.Player.GetComponentInChildren<CameraTarget>();
            if (Target)
            {
                return Target.transform;
            }
            else
            {
                return IPlayerCentric.Player.transform;
            }
        }

        //A bit sketchy since this can also just be called outside of this script.
        /*
            Mostly just here so that it can be called within visual scripting
        */

        public void AssignControlToPlayer(UnitSystem.GameUnit playerUnit)
        {
            playerUnit.PlayerTakeControl();
            IPlayerCentric.Player=playerUnit.gameObject;
        }

        public void RemoveControlToPlayer(UnitSystem.GameUnit playerUnit)
        {
            playerUnit.PlayerRemoveControl();
            IPlayerCentric.Player=null;
        }

    }
}