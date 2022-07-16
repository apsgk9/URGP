using System.Collections;
using System.Collections.Generic;
using Service;
using UnityEngine;

public class GameServiceBehaviour<T> : MonoBehaviour , IGameService where T: GameServiceBehaviour<T>
{
    virtual protected void Awake()
    {        
        ServiceLocator.Current.Register<T>(this as T);
    }

    virtual protected void OnDestroy()
    {        
        ServiceLocator.Current.Unregister<T>();        
    }
}
