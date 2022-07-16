using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "SystemSettings", menuName = "ScriptableObjects/Settings/SystemSettings", order = 0)]
public class SystemSettings : ScriptableObject
{
    [Min(0)]
    public float DefaultSceneFadeTransitionDuration = 2.0f;
    [Min(0)]
    public float DefaultInitialWaitTimeBeforeTransitionDuration = 0.5f;
}