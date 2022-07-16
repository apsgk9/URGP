using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "Settings", menuName = "ScriptableObjects/Settings/Settings", order = 0)]
public class Settings: ScriptableObject
{
    public InputSettings InputSettings;
    public MenuSettings MenuSettings;
    public SystemSettings SystemSettings;

}
