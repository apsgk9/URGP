using System;
using System.Collections;
using System.Collections.Generic;
using Service;
using UI.MenuController;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
[RequireComponent(typeof(CanvasGroup))]
public class ShiftableMenuPanel : MenuShiftable, IScrollHandler, IPointerExitHandler, IPointerEnterHandler
{
    [Header("ShiftableMenuPanel")]
    public MenuController MenuControllerToFocus;
    public bool FocusOnHover = true;
    public bool AllowSelectionWhenUnfocused =false;
    public bool DisableDeselection =false;
    private CanvasGroup _canvasGroup;
    [SerializeField]
    private ScrollRectMenu _ScrollRectMenu;
    private bool previousInteractable;
    protected override void Awake()
    {
        base.Awake();
        _canvasGroup= GetComponent<CanvasGroup>();
        if(transform.childCount>0 && _ScrollRectMenu==null)
        {
            _ScrollRectMenu = transform.GetChild(0).GetComponent<ScrollRectMenu>();
        }
        
        ChangeInInteractable(interactable);
        if(DisableDeselection)
        {
            Select();            
        }
    }
    private void Update()
    {
        if(previousInteractable!=interactable)
        {
            previousInteractable=interactable;
            ChangeInInteractable(interactable);
        }        
    }

    private void ChangeInInteractable(bool newInteractable)
    {
        if(_canvasGroup==null)
            return;
        _canvasGroup.blocksRaycasts=newInteractable;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    

    protected override void OnDisable()
    {
        base.OnDisable();
        DeregisterNavigationCommands();
    }

    

    private void OnPressNavigate(InputAction.CallbackContext context)
    {
        //Debug.Log("NAVIGATE: "+gameObject.name);
        if(interactable)
        {
            Vector2 Navigate= context.ReadValue<Vector2>();
            if(_MenuControllerParent)
            {
                //Reverse To Have correct Context. Might not be accurate for Vertical. Only Test Horizontal
                _MenuControllerParent.OnMove(-Navigate);
            }
        }
    }




    public void OnPointerEnter(PointerEventData eventData)
    {
        if(DisableDeselection)
            return;

        if (CanInteractWith())
        {
            if(_MenuControllerParent)
            {
                _MenuControllerParent.OnEnterShiftable(this);
            }
            else
            {
                Select();
            }        
            

            
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(DisableDeselection)
            return;

        if (CanInteractWith())
        {
            if(_ScrollRectMenu!=null && _ScrollRectMenu.BeingDragged)
                return;

            if(_MenuControllerParent)
            {
                _MenuControllerParent.OnEnterShiftable(this);
            }
            else
            {
                Deselect();
            }
        }
    }


    public override void Select()
    {
        if(Selected==true && DisableDeselection==false)
            return;
        if (MenuControllerToFocus)
        {
            Selected=true;
            RegisterNavigationCommands();
            MenuControllerToFocus.SetFocus(true);
        }
    }

    public override void Deselect()
    {
        if(Selected==false)
            return;
            
        if (MenuControllerToFocus)
        {
            Selected=false;
            DeregisterNavigationCommands();
            MenuControllerToFocus.SetFocus(false);
        }

        
    }



    public void OnScroll(PointerEventData eventData)
    {
        if (interactable)
        {
            MenuControllerToFocus.OnMove(eventData.scrollDelta.y);
        }
    }

    private bool CanInteractWith()
    {
        bool CanSelect= 
        Selected 
        || (_MenuControllerParent==null && AllowSelectionWhenUnfocused)
        || (_MenuControllerParent!=null && AllowSelectionWhenUnfocused && !Selected && _MenuControllerParent.currentIndexSelected!=transform.GetSiblingIndex());
        bool CursorState= Cursor.visible && Cursor.lockState != CursorLockMode.Locked;
        return interactable && FocusOnHover && CursorState && CanSelect;
    }

    private void RegisterNavigationCommands()
    {
        if (UserInput.CanAccess)
        {
            //Debug.Log("REGISTER: "+gameObject.name);
            UserInput.Instance.PlayerInputActions.MenuControls.Navigate.performed += OnPressNavigate;
            UserInput.Instance.PlayerInputActions.LimitedMenuControls.Navigate.performed += OnPressNavigate;
        }
    }

    private void DeregisterNavigationCommands()
    {
        if (UserInput.CanAccess)
        {
            //Debug.Log("DEREGISTER: "+gameObject.name);
            UserInput.Instance.PlayerInputActions.MenuControls.Navigate.performed -= OnPressNavigate;
            UserInput.Instance.PlayerInputActions.LimitedMenuControls.Navigate.performed -= OnPressNavigate;
        }
    }

    public bool ShouldScrollRectDrag()
    {
        if(FocusOnHover)
        {
            return CanInteractWith();
        }
        return false;
    }
}
