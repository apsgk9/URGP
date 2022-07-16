using System.Collections;
using System.Collections.Generic;
using GameMode;
using UnityEngine;

[RequireComponent(typeof(FreeRoamMode))]
public class FreeRoamModeInitializer : MonoBehaviour
{
    public UnityEngine.AddressableAssets.AssetReference DefaultCharacterAsset;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        while(GameManager.Instance.HasInitialized==false)
        {
            yield return null;
        }

        var _FreeRoamMode= GetComponent<FreeRoamMode>();       

        StartCoroutine(_FreeRoamMode.LoadUI());

        while(_FreeRoamMode.PlayerUIInitialized==false)
        {
            yield return null;
        }

        StartCoroutine(_FreeRoamMode.LoadCharacter(DefaultCharacterAsset));

        while(_FreeRoamMode.PlayerCharacterInitialized==false)
        {
            yield return null;
        }

        _FreeRoamMode.SpawnCharacterInASpawnPosition();

        //Keep attempting to begin until game is ready.

        while(_FreeRoamMode.isRunning==false)
        {
            _FreeRoamMode.Begin();
            yield return null;
        }
        
    }

}
