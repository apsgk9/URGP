using System;
using UnityEngine;

//Possible Have a hasset option, since you might access it without setting it up
[Serializable]
public class AnimatorParamaterList
{
    [SerializeField]
    public Animator Animator;
    [SerializeField]
    public string Name;
    [SerializeField]
    public int Hash;
    [SerializeField]
    public AnimatorControllerParameterType Type;
    [HideInInspector]
    public int m_Index;

    public override string ToString()
    {
        return Name;
    }
}