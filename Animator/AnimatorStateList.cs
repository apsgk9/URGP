using System;
using UnityEngine;

//Possible Have a hasset option, since you might access it without setting it up
[Serializable]
public class AnimatorStateList
{
    [SerializeField]
    public Animator Animator;
    [SerializeField]
    public string Name;
    [SerializeField] [Min(0)]
    public int Layer;
    [HideInInspector]
    public int m_motionIndex;

    public override string ToString()
    {
        return Name;
    }
}
