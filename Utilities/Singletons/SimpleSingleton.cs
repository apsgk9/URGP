using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
    This one doesn't spawn a new one if not needed
*/
public abstract class SimpleSingleton<T> : MonoBehaviour where T:MonoBehaviour
{
    public static T Instance;

    virtual protected void Awake()
    {
        if(Instance!=null)
        {
            Destroy(this);
        }
        else
        {
            Instance=GetComponent<T>();
        }
    }

    virtual protected void OnDestroy()
    {
        if(Instance==this)
        {
            Instance=null;
        }
    }
}
