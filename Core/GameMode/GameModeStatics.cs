using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameStatics
{
    public class GameModeStatics : Singleton<StaticFunctions>
    {
        public static void SetPlayer(GameObject PlayerGO)
        {
            GameMode.IPlayerCentric.Player = PlayerGO;
        }

        public static GameObject GetPlayer()
        {
            return GameMode.IPlayerCentric.Player;
        }
    }
}