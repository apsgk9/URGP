using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "MenuSettings", menuName = "ScriptableObjects/Settings/MenuSettings", order = 2)]
public class MenuSettings : ScriptableObject
{
    public float MenuSpeed =0.1f;
}