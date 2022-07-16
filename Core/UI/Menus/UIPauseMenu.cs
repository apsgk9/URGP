using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;

public class UIPauseMenu : MonoBehaviour
{
    public GameObject PauseMenu;
    private void Update()
    {        
        PauseMenu.SetActive(GameState.isPaused);        
    }
}

