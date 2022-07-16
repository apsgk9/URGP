using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class SceneTransitionTest : MonoBehaviour
{

    public AssetReference SceneToLoad;

    private void OnTriggerEnter(Collider other)
    {
        if(GameMode.IPlayerCentric.Player== other.gameObject)
        {
            var GameUnit=GameMode.IPlayerCentric.Player.GetComponent<UnitSystem.GameUnit>();
            GameUnit.PlayerRemoveControl();
            GameTransitionManager.Instance.LoadScene(SceneToLoad);
        }   
    }

    
}
