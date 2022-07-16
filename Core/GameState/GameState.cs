
using System;
using System.Collections.Generic;
using UnityEngine;

/*
    Holds what State the game currently is. Is useful for things like pausing the game.
    Scripts should refer to this to know state the game or what its currently doing like if the application is qutting.
*/
public class GameState : Singleton<GameState>
{
    private const int FramesToInspect = 10;
    private static float _baseTickRate;
    [SerializeField]  [ReadOnly]
    private float AvgFramerate;
    [SerializeField]  [ReadOnly]
    private Queue<float>framerates = new Queue<float>();
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Reset()
    {
        _isPaused=false;
    }
    private void Awake()
    {
        GameObject.DontDestroyOnLoad(this);
    }
    public static bool isPaused{get{return _isPaused;}set{HandlePause(value);}}

    private static bool _isPaused;

    private static void HandlePause(bool newPauseValue)
    {
        if(_isPaused==false && newPauseValue==true) //Pausing
        {
            ChangeInPause?.Invoke(newPauseValue);
        }
        else if(_isPaused==true && newPauseValue==false) //UnPausing
        {
            ChangeInPause?.Invoke(newPauseValue);
        }
        _isPaused=newPauseValue;
    }

    public static Action<bool> ChangeInPause;

    public static bool isApplicationQuitting{get{return applicationIsQuitting;}}

    protected override void OnApplicationQuit()
    {
        base.OnApplicationQuit();        
    }

    public static float GetTickRate()
    {
        return _baseTickRate;
    }

    private void Update()
    {
        UpdateFramerate();
        if(AvgFramerate<=60)
        {
            _baseTickRate=60;
        }
        else if(AvgFramerate>=60 && AvgFramerate<90)
        {
            _baseTickRate=90;
        }
        else if(AvgFramerate>=120)
        {
            _baseTickRate=120;
        }

    }

    private void UpdateFramerate()
    {
        if (framerates.Count > FramesToInspect)
        {
            framerates.Dequeue();
        }
        float currentFrameRate = 1 / Time.unscaledDeltaTime;
        framerates.Enqueue(currentFrameRate);
        float total = 0;
        foreach (float f in framerates.ToArray())
        {
            total += f;
        }
        AvgFramerate= total/ framerates.Count;

    }
}
