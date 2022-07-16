using System;
using Events;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;

public class UIQuickMenu : MonoBehaviour
{
    //NOTE-- This is kinda confusing since zooming in and out is arbitrary due to the fact that it can be inversed
    public GameObject QuickMenu;
    public GameEvent ZoomIn;
    public GameEvent ZoomOut;
    public bool zoomingIN;
    public bool zoomingOut;
    private void Awake()
    {
        QuickMenu.SetActive(false);        
    }
    private void OnEnable()
    {
        if(UserInput.CanAccess)
        {
            UserInput.Instance.PlayerInputActions.PlayerControls.QuickMenuKey.performed += HandleQuickMenuPressed;
            UserInput.Instance.PlayerInputActions.PlayerControls.QuickMenuKey.canceled += HandleQuickMenuReleased;

            UserInput.Instance.PlayerInputActions.PlayerControls.GamePadZoomIn.performed += HandleZoomInPressed;
            UserInput.Instance.PlayerInputActions.PlayerControls.GamePadZoomIn.canceled += HandleZoomInReleased;

            UserInput.Instance.PlayerInputActions.PlayerControls.GamePadZoomOut.performed += HandleZoomOutPressed;
            UserInput.Instance.PlayerInputActions.PlayerControls.GamePadZoomOut.canceled += HandleZoomOutReleased;

            
        }
    }

    

    private void OnDisable()
    {
        if(UserInput.CanAccess)
        {
            UserInput.Instance.PlayerInputActions.PlayerControls.QuickMenuKey.performed -= HandleQuickMenuPressed;
            UserInput.Instance.PlayerInputActions.PlayerControls.QuickMenuKey.canceled -= HandleQuickMenuReleased;

            UserInput.Instance.PlayerInputActions.PlayerControls.GamePadZoomIn.performed -= HandleZoomInPressed;
            UserInput.Instance.PlayerInputActions.PlayerControls.GamePadZoomIn.canceled -= HandleZoomInReleased;

            UserInput.Instance.PlayerInputActions.PlayerControls.GamePadZoomOut.performed -= HandleZoomOutPressed;
            UserInput.Instance.PlayerInputActions.PlayerControls.GamePadZoomOut.canceled -= HandleZoomOutReleased;
        }        
    }

    private void OnDestroy()
    {
        if(UserInput.CanAccess)
        {
            UserInput.Instance.PlayerInputActions.PlayerControls.QuickMenuKey.performed -= HandleQuickMenuPressed;
            UserInput.Instance.PlayerInputActions.PlayerControls.QuickMenuKey.canceled -= HandleQuickMenuReleased;

            UserInput.Instance.PlayerInputActions.PlayerControls.GamePadZoomIn.performed -= HandleZoomInPressed;
            UserInput.Instance.PlayerInputActions.PlayerControls.GamePadZoomIn.canceled -= HandleZoomInReleased;

            UserInput.Instance.PlayerInputActions.PlayerControls.GamePadZoomOut.performed -= HandleZoomOutPressed;
            UserInput.Instance.PlayerInputActions.PlayerControls.GamePadZoomOut.canceled -= HandleZoomOutReleased;
        }
    }

    private void Update()
    {
        if(GameState.isPaused)
        {
            QuickMenu.SetActive(false);
        }
        if(QuickMenu.activeInHierarchy)
        {            
            if(zoomingIN)
            {
                //Debug.Log("zoomingIN");
                //Debug.Log(ZoomIn==null);
                ZoomIn.Raise();
            }
            else if(zoomingOut)
            {
                //Debug.Log("zoomingOut");
                //Debug.Log(ZoomOut==null);
                ZoomOut.Raise();
            }
        }
        
    }

    private void HandleQuickMenuReleased(InputAction.CallbackContext obj)
    {
        if(GameState.isPaused)
            return;
        QuickMenu.SetActive(false);
    }

     
    private void HandleQuickMenuPressed(InputAction.CallbackContext context)
    {
        if(GameState.isPaused)
            return;
        QuickMenu.SetActive(true);
    }

    private void HandleZoomInReleased(InputAction.CallbackContext obj)
    {
        zoomingIN=false;
    }

    private void HandleZoomInPressed(InputAction.CallbackContext obj)
    {
        if(zoomingOut)
        {
            zoomingOut=false;
        }
        zoomingIN=true;
    }

    private void HandleZoomOutReleased(InputAction.CallbackContext obj)
    {
        zoomingOut=false;
    }

    private void HandleZoomOutPressed(InputAction.CallbackContext obj)
    {
        if(zoomingIN)
        {
            zoomingIN=false;
        }
        zoomingOut=true;
    }
}
