

using UnityEngine;
using static LocomotionEnmus;

public abstract class CharacterModifier : MonoBehaviour
{
    public int priority=0;

    protected bool isActive=false;

    private void OnEnable()
    {
        isActive=true;
    }
    private void OnDisable()
    {
        isActive=false;        
    }
    public abstract void  Handle(Character ctx);
}