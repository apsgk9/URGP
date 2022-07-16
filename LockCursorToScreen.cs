using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LockCursorToScreen : MonoBehaviour
{
    [SerializeField]
    private bool isCursorLocked=true;
    [SerializeField]
    private bool isConfined;
    
    public PlayerInputActions PlayerInputActions;

    private void Awake()
    {
        
        PlayerInputActions = new PlayerInputActions();
        PlayerInputActions.Enable();
        PlayerInputActions.UserControls.ConfineCursor.performed+=ToggleCursorLocked;        
        PlayerInputActions.UserControls.ConfineCursor.canceled+=ToggleCursorLocked;
        UpdateLock();         
    }
    private void OnDestroy()
    {
        PlayerInputActions.UserControls.ConfineCursor.performed-=ToggleCursorLocked;
        PlayerInputActions.UserControls.ConfineCursor.canceled-=ToggleCursorLocked;
    }

    private void ToggleCursorLocked(InputAction.CallbackContext obj)
    {
        isConfined=!isConfined;
        UpdateLock(); 
    }

    private void UpdateLock()
    {
        if(isCursorLocked)
        {
            if(isConfined)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible=true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Confined;
            }
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible=true;
        }        
    }
}
