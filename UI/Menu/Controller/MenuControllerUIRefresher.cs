using System;
using System.Collections;
using System.Collections.Generic;
using UI.MenuController;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class MenuControllerUIRefresher : MonoBehaviour
{
    MenuController _menuController;
    [SerializeField] [ReadOnly]
    private float _previousSizeDeltaY;
    private float _previousSizeDeltaX;
    private int _previousCount=-1;
    private int _previousContainerSize=-1;
    private RectTransform rectTransform;
    public HorizontalOrVerticalLayoutGroup HVLayoutGroup;
    [SerializeField] [ReadOnly]
    private RectOffset _previousLayoutGroupPadding;

    private void Awake()
    {
        CheckProperties();
        SetPreviousPadding();
        RebuildLayoutGroup();
    }

    private void Start()
    {
        CheckProperties();
    }
    private void OnValidate()
    {
        
        CheckProperties();
        if(CannotRun())
            return;
        if(_menuController.GetOrientation()== Orientation.Horizontal)
        {
            SetUpHorizontalRectTransformProperties();
        }
        else if(_menuController.GetOrientation()== Orientation.Vertical)
        {
            SetUpVerticalRectTransformProperties();
        }
    }

    private void CheckProperties()
    {   
        if (HVLayoutGroup == null)
        {
            HVLayoutGroup = GetComponent<HorizontalOrVerticalLayoutGroup>();
        }
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }
        if (_menuController == null)
        {
            _menuController = GetComponent<MenuController>();
        }
    }

    private void Update()
    {
        RefreshCheck();
    }
    private void LateUpdate()
    {
        RefreshCheck();
    }

    private void RefreshCheck()
    {
        bool change = ChangeInUI();
        if (change)
        {
            if (HVLayoutGroup)
            {
                RebuildLayoutGroup();
            }
            _menuController.RefreshUI();
        }
    }

    private void RebuildLayoutGroup()
    {
        //For Larger Menus, its best to keep LayoutGroupOff, but for things that turn on and off, best to turn it on or else it bugs out.
        bool initial=HVLayoutGroup.enabled;
        if (HVLayoutGroup.enabled == false)
        {
            HVLayoutGroup.enabled = true;
        }
        else
        {
            HVLayoutGroup.enabled = false;
            HVLayoutGroup.enabled = true;
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(this.rectTransform);
        HVLayoutGroup.enabled = initial;
    }

    private bool CannotRun()
    {
        return HVLayoutGroup == null || rectTransform == null || _menuController == null;
    }

    private bool ChangeInUI()
    {
        CheckProperties();
        if(CannotRun())
            return false;
        
        bool success=false;
        if( (HVLayoutGroup.padding.bottom!=_previousLayoutGroupPadding.bottom) ||
            (HVLayoutGroup.padding.top!=_previousLayoutGroupPadding.top) ||
            (HVLayoutGroup.padding.right!=_previousLayoutGroupPadding.right) ||
            (HVLayoutGroup.padding.left!=_previousLayoutGroupPadding.left)
        )
        {
            SetPreviousPadding();
            success = true;
        }

        bool differentDeltaY = _previousSizeDeltaY!=rectTransform.sizeDelta.y;
        bool differentDeltaX = _previousSizeDeltaX!=rectTransform.sizeDelta.x;

        if(_menuController.GetOrientation()== Orientation.Horizontal)
        {
            if(differentDeltaY)
            {
                SetUpHorizontalRectTransformProperties();
                success= true;
            }
        }
        else if(_menuController.GetOrientation()== Orientation.Vertical)
        {
            if(differentDeltaX)
            {
                SetUpVerticalRectTransformProperties();
                success= true;
            }
        }
        _previousSizeDeltaY=rectTransform.sizeDelta.y;
        _previousSizeDeltaX=rectTransform.sizeDelta.x;
        if(!(_menuController is MenuControllerContainer))
            return success;
        
        //MenuConctrollerContainer Starting From Here
        var containerController=_menuController as MenuControllerContainer;
        HandleSizeDeltaChange(containerController);

        bool differentCount = _previousCount!=containerController.GetCount();
        bool differentContainerSize = _previousContainerSize!=containerController.GetContainerSize();
        
        _previousCount=containerController.GetCount();
        _previousContainerSize=containerController.GetContainerSize();
        
        if(differentDeltaX || differentDeltaY || differentCount || differentContainerSize)
        {
            //Debug.Log("3:"+differentDeltaX + " || " + differentDeltaY + " || " + differentCount + " || " + differentContainerSize);
            HandleSizeDeltaChange(containerController);
            
            success= true;
        }

        return success;
    }

    private void SetPreviousPadding()
    {
        _previousLayoutGroupPadding.bottom = HVLayoutGroup.padding.bottom;
        _previousLayoutGroupPadding.top = HVLayoutGroup.padding.top;
        _previousLayoutGroupPadding.right = HVLayoutGroup.padding.right;
        _previousLayoutGroupPadding.left = HVLayoutGroup.padding.left;
    }

    private void SetUpHorizontalRectTransformProperties()
    {
        rectTransform.anchorMin = new Vector2(0.0f,0.5f);
        rectTransform.anchorMax = new Vector2(1f,0.5f);
        rectTransform.pivot = new Vector2(0.5f,0.5f);
    }

    private void SetUpVerticalRectTransformProperties()
    {
        rectTransform.anchorMin = new Vector2(0.5f,0.0f);
        rectTransform.anchorMax = new Vector2(0.5f,1);
        rectTransform.pivot = new Vector2(0.5f,0.5f);
    }
    /*
        Changes the Size of the rect that can be clicked on. Useful for scroll rect
    */
    private void HandleSizeDeltaChange(MenuControllerContainer containerController)
    {
        if (_menuController.GetOrientation() == Orientation.Vertical)
        {
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x,
            ((containerController.GetCount() - containerController.GetContainerSize()) * containerController.GetPixelDimension()));
        }
        else
        {
            rectTransform.sizeDelta = new Vector2(((containerController.GetCount() - containerController.GetContainerSize()) * containerController.GetPixelDimension()),
            rectTransform.sizeDelta.y);

        }
    }
}
