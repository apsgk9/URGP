using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnitSystem
{
    //All Game Units can be controlled by the player/user.
    public abstract class  GameUnit : MonoBehaviour
    {
        public UnitParameters unitParameters;
        public class UnitArgs: EventArgs
        {
            public UnitParameters unitParameters;
            public GameUnit unit;
        }
        

        protected bool _PlayerControlled;
        public bool IsPlayerInControl() => _PlayerControlled;
        public abstract void PlayerTakeControl();
        public abstract void PlayerRemoveControl();

        
        public static event EventHandler<UnitArgs> OnGroupChange;
        public static event EventHandler<UnitArgs> PlayerGroupingChange;
        protected void PlayerGroupingHasChanged()
        {
            GameUnit.PlayerGroupingChange?.Invoke(this,new UnitArgs{unitParameters=unitParameters,unit=this});
        }
        protected void GroupingHasChanged()
        {
            GameUnit.OnGroupChange?.Invoke(this,new UnitArgs{unitParameters=unitParameters,unit=this});
        }
    }
}