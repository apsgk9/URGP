using System.Collections;
using GameStatics;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
    --IMPORTANT--
    THIS IS THE SCRIPT THAT INITIALIZES THE ENTIRE GAME
*/
public class InitializationStateMachine
{
    private const string InitGameStateMachinePath = "Initialization/GameStateMachine";
    private const string MainSceneName = "Scenes/Main";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void Init()
    {
        GameObject StaticFunctionsSingleton = new GameObject();
        StaticFunctionsSingleton.name=typeof(StaticFunctions).ToString();
        var StaticFunctionsClass =StaticFunctionsSingleton.AddComponent<StaticFunctions>();
        GameObject.DontDestroyOnLoad(StaticFunctionsSingleton);
        StaticFunctionsClass.StartCoroutine(InitializeOnLoad());
    }
    public static IEnumerator InitializeOnLoad()
    {
        //if(!GameObject.FindObjectOfType<ConsoleToGUI>())
        //{
        //    var consoleGameObject = new GameObject("ConsoleToGUI");
        //    var consoleref=consoleGameObject.AddComponent<ConsoleToGUI>();
        //}
        
        //Debug.Log("Initializing");

        //Create Initial Singletons
            

        while(!Resources.Load(InitGameStateMachinePath, typeof(GameObject)))
        {
            yield return null;
        }

        /*
            Probably need to have a main scene.
        */
        //Debug.Log("START");
        //var mainScene=SceneManager.GetSceneByName(MainSceneName);
        //if(!mainScene.IsValid())
        //{
        //    Debug.Log("MAIN DOES NOT EXIST");
        //    SceneManager.LoadSceneAsync(MainSceneName,LoadSceneMode.Additive);
        //    while(!SceneManager.GetSceneByName(MainSceneName).IsValid())
        //    {
        //        yield return null;
        //    }
        //}
        GameObject gameManagerGameObject = GameObject.Instantiate(Resources.Load(InitGameStateMachinePath, typeof(GameObject))) as GameObject;
        gameManagerGameObject.name="GameStateMachine";
        
        GameObject.DontDestroyOnLoad(gameManagerGameObject.gameObject);
        
    }
}