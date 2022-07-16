using UnityEngine;
using UnityEngine.AddressableAssets;

//currently unused, will likely use again with more ui usage at some point
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/UIList", order = 1)]
public class UIObjects : ScriptableObject
{
    public AssetReferenceGameObject PauseMenuGameObject;
    public AssetReferenceGameObject QuickMenuGameObject;
}