using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "AssetReferenceSO", menuName = "ScriptableObjects/AssetReferenceSO", order = 3)]
public class AssetReferenceSO : ScriptableObject
{    
    public string AssetName;
    public AssetReference AssetReference;

}
