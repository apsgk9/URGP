using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

public class NotifyOnDestroyNonAssetReference : MonoBehaviour
{
    public AsyncOperationHandle<GameObject> OperationHandle;

    public event Action<NotifyOnDestroyNonAssetReference> Destroyed;

    public void OnDestroy()
    {
        Destroyed?.Invoke(this);
    }
}
