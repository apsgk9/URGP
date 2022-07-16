
/*

    Describes the game mode as having a player character that the user can control.
    So a first person, third person game. Racing game, figher pilot game, etc.
    A non player centric game mode would be something like an rts or a card game
    where they isn't a specific unit that the player controls to move.

*/
using UnitSystem;
using UnityEngine;
using Service;
using System;
using Unity.VisualScripting;

namespace GameMode
{
    /*
        Figure out if we can Assign Control to multiple units as the player or not.
    */

    public interface IPlayerCentric
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void Reset()
        {            
            _player=null;
        }
        private static GameObject _player { get; set; }
        public static GameObject Player { get {return _player;}        
        set
            {
                if(_player!=value) 
                    PlayerHasChangedTo?.Invoke(_player,value);
                _player=value;
            }  
        }
        //<Old PLayer, New Player>
        static Action<GameObject,GameObject> PlayerHasChangedTo { get; set; }
        //Call this if you want the camera to react to a new player switch
        static Action<GameObject,GameObject> SwitchCameraToNewPlayer { get; set; }

        static Transform GetCameraTargetOnPlayer;

        //If control is assigned, it should become the new main player
        void AssignControlToPlayer(UnitSystem.GameUnit playerCharacter);
        /*
            Deassigning control to a player doesn't necessarily mean they don't become the main player anymore
            You might want to prevent control during cutscene.
        */
        void RemoveControlToPlayer(UnitSystem.GameUnit playerCharacter);
    }
}