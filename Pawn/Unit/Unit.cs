using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controller;
using UnityEngine;

namespace UnitSystem
{
    public class Unit : GameUnit
    {
        [ReadOnly]
        public Pawn Pawn;
        
        [ReadOnly]

        public IController Controller;
#region Anchors
        /*
            These are used to indicate where certain UI elements or status effects should be placed on the unit.
        */
        [Header("Anchors")]
        [SerializeField]

        private Transform _topAnchor;
        public Transform TopAnchor
        {
            get 
            {   if(_topAnchor!=null)
                    return _topAnchor;
                return transform;
            }
            set {_topAnchor=value;}
        }
        [SerializeField]
        private Transform _centerAnchor;
        public Transform CenterAnchor
        {
            get 
            {   if(_centerAnchor!=null)
                    return _centerAnchor;
                return transform;
            }
            set {_centerAnchor=value;}
        }
        [SerializeField]
        private Transform _bottomAnchor;
        
        public Transform BottomAnchor
        {
            get 
            {   if(_bottomAnchor!=null)
                    return _bottomAnchor;
                return transform;
            }
            set {_bottomAnchor=value;}
        }
        [SerializeField] private List<string> _Tags;

        public List<string> Tags => _Tags;

        #endregion

        private void OnValidate()
        {
            GetUnitComponents();
        }

        private void ValidateTagList()
        {
            if(_Tags.Count != _Tags.Distinct().Count())
            {
                Debug.LogError("Duplicate Tags exist on Unit: "+transform.gameObject);                
            }
        }

        private void Start()
        {            
            ValidateTagList();

            GetUnitComponents();

            if (Pawn == null)
            {
                Debug.LogError("Pawn is null.");
            }
            if (Controller == null)
            {
                Debug.LogError("Controller is null.");
            }
        }
        private void GetUnitComponents()
        {
            if (Pawn == null)
                Pawn = GetComponent<Pawn>();
            if (Controller == null)
                Controller = GetComponent<IController>();
        }
        [ContextMenu("PlayerRemoveControl")]

        public override void PlayerRemoveControl()
        {
            if(Controller is PlayerCharacterController)
            {
                var PC = Controller as PlayerCharacterController;
                PC.inControl=false;
                _PlayerControlled=false;

                //Please Do not remove from playergroup
                //unitParameters.ClearGroup();
                //GroupingHasChanged();
            }
        }
        [ContextMenu("PlayerTakeControl")]

        public override void PlayerTakeControl()
        {
            if(Controller is PlayerCharacterController)
            {
                var PC = Controller as PlayerCharacterController;
                PC.inControl=true;          
                _PlayerControlled=true;

                //Since this is taken control by the player, it is now in the playergroup
                unitParameters.SetToPlayerGroup();
                PlayerGroupingHasChanged();
            }
        }

        [ContextMenu("AddToPlayerGroup")]

        public void AddToPlayerGroup()
        {
            if(Controller is PlayerCharacterController)
            {
                unitParameters.SetToPlayerGroup();
                PlayerGroupingHasChanged();
            }
        }

        [ContextMenu("RemoveFromPlayerGroup")]

        public void RemoveFromPlayerGroup()
        {
            if(Controller is PlayerCharacterController)
            {
                var PC = Controller as PlayerCharacterController;
                if(PC.inControl)
                    return;
                unitParameters.ClearGroup();
                PlayerGroupingHasChanged();
            }
        }
        
    }
}
