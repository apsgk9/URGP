using System;
using UnityEngine;

public class Timer
{
    public float time {get;private set;}
    public float threshold;
    public bool Activated=>time>=threshold;
    public Timer(float thresholdInput)
    {
        threshold=thresholdInput;
    }
    public void Tick()
    {
        time+=Time.deltaTime;
    }public void FixedTick()
    {
        time+=Time.fixedDeltaTime;
    }
    public void ResetTimer()
    {
        time=0;
    }
    public void FinishTimer()
    {
        time=threshold+1;
    }
}