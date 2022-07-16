using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementHistory
{
    private LinkedList<float> _History= new LinkedList<float>();
    private int historyMaxLength;
    private float Sum;
    private float _average;

    public MovementHistory(int length)
    {
        historyMaxLength=length;
    }


    // Update is called once per frame
    public void Tick(float input,bool applyMultiplier=false)
    {
        _History.AddFirst(input);
        var multiplier = 1f;
        if (applyMultiplier)
        {
            multiplier = 0.5f;
        }
        while (_History.Count > historyMaxLength * multiplier)
        {
            _History.RemoveLast();
        }
    }
    public float Average()
    {
        Sum=0f;
        foreach(float number in _History)
        {
            Sum+=Mathf.Abs(number);
        }
        _average=Sum/((float)_History.Count);
        return _average;
    }
}
