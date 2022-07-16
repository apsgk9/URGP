using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnitSystem
{
    [Serializable]
    //Paramets to describe a base Unit
    public class UnitParameters
    {
        //Describes which Group this unit belongs to. 
        public const string PLAYER_TAG = "Player";
        public static string EMPTY_TAG ="";
        public string Group {get;private set;}
        public string PreviousGroup {get;private set;}

        [ReadOnly]
        public List<string> FactionList;

        public void AddFaction(string tagToAdd)
        {
            if (FactionList.Contains(tagToAdd))
            {
                Debug.LogWarning($"Unit already has {tagToAdd} faction");
            }
            else
            {
                FactionList.Add(tagToAdd);
            }
        }

        public void RemoveFaction(string tagToRemove)
        {
            if (FactionList.Contains(tagToRemove))
            {
                FactionList.Remove(tagToRemove);
            }
            else
            {
                Debug.LogWarning($"Unit does not have {tagToRemove} faction");
            }
        }

        public void SetToPlayerGroup()
        {
            PreviousGroup=Group;
            Group=PLAYER_TAG;
        }
        
        public void SetGroup(string GroupName)
        {
            PreviousGroup=Group;
            Group=GroupName;
        }
        public void ClearGroup()
        {
            PreviousGroup=Group;
            Group=EMPTY_TAG;
        }
    }
}
