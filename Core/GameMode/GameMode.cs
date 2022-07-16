using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
    Handles how the game should be run for that particular instance
*/
namespace GameMode
{

    public abstract class GameModeBase<T,GAMEMODECLASS> : SimpleSingleton<GameModeBase<T,GAMEMODECLASS>> where T : GameModeBase<T,GAMEMODECLASS>
    where GAMEMODECLASS: Component
    {
        //public static void Create()
        //{
        //    if (GameModeBase<T,GAMEMODECLASS>.Instance != null)
        //        return;
        //    var newObj = new GameObject();
        //    newObj.AddComponent<GAMEMODECLASS>();
        //    newObj.name = GetSceneName();
        //    SetupGameModeScene();
        //    MoveToScene(newObj);
        //}
        public bool isRunning {get; protected set;}

        public abstract bool isReady();
        
        /*
            This should start the GameMode if everything is setup properly.
            You have to setup it up yourself since some assets might be loaded in seperately.
        */
        public abstract void Begin();
        /*
            This should end the gamemode. It should remove everything that is associated with that mode
            such as certain UI elements or specific scripts specific to that mode.
        */
        public abstract void End();
        

        private static void SetupGameModeScene()
        {
            var GameModeScene = SceneManager.GetSceneByName(GetSceneName());
            if (!GameModeScene.IsValid())
            {
                SceneManager.CreateScene(GetSceneName());
            }
        }

        private static void DestroyGameModeScene()
        {
            var GameModeScene = SceneManager.GetSceneByName(GetSceneName());
            if (GameModeScene.IsValid())
            {
                SceneManager.UnloadSceneAsync(GameModeScene);
            }
        }

        public static void MoveToScene(GameObject GO)
        {
            SceneManager.MoveGameObjectToScene(GO, SceneManager.GetSceneByName(GetSceneName()));
        }

        public static string GetSceneName()
        {
            return typeof(GAMEMODECLASS).FullName;
        }



    }

}