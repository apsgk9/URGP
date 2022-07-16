using System;
using System.Collections;
using System.Collections.Generic;
using Service;
using UnityEngine;


public class GameManager : Singleton<GameManager>
{

    [SerializeField]
    private GameInitialization GameInitializer;
    public bool HasInitialized {get{return GameInitializer.HasInitialized;}}

    private void Start()
    {
        InitializeSystems();
    }
    public void InitializeSystems()
    {
        if(GameInitializer==null)
        {
            GameInitializer=new GameInitialization();
        }
        GameInitializer.InitializeSystems();
    }
}
/*
Free Roam: This mode is the default mode. It is the most flexible and done in realtime. 
Character movements and other  actions are not limited in any way. In essence its an open sandbox

TurnBasedBattle: Characters and other entities that follow the game flow are controlled by an overarching master. 
Their movements and sequences are limited to whatever this manager says.
*/
/*
public enum GameMode
{
    FreeRoam,
    TurnBasedBattle
}*/