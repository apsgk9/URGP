using System.Collections;
using System.Collections.Generic;
using Service;
using UnityEngine;
using UnityEngine.InputSystem.UI;

public class MenuInputSystemController : GameServiceBehaviour<MenuInputSystemController>
{
    [SerializeField]
    private InputSystemUIInputModule _MenuInputs;
    public InputSystemUIInputModule MenuInputs {get{return _MenuInputs;}}
    [SerializeField]
    private InputSystemUIInputModule _LimitedMenuInputs;

    public InputSystemUIInputModule LimitedMenuInputs {get{return _LimitedMenuInputs;}}
    private bool _previousfocusedInMenu;
    protected override void Awake()
    {
        UpdateEnables();
        base.Awake();        
    }

    private void Update()
    {
        if(UIManager.Instance!=null && _previousfocusedInMenu!=UIManager.Instance.UserFocusedInMenu)
        {
            UpdateEnables();
        }

    }

    private void UpdateEnables()
    {
        _MenuInputs.enabled = UIManager.Instance.UserFocusedInMenu;
        _LimitedMenuInputs.enabled = !UIManager.Instance.UserFocusedInMenu;
        _previousfocusedInMenu = UIManager.Instance.UserFocusedInMenu;
    }
}
