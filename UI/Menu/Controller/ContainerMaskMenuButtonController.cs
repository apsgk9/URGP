using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI.MenuController;

[ExecuteAlways]
[RequireComponent(typeof(RectMask2D))]
public class ContainerMaskMenuButtonController : MonoBehaviour
{
    [Min(0)]
    public int Padding=0;
    private int previousPadding;
    [SerializeField] [ReadOnly]
    MenuControllerContainer MenuContainer;
    RectMask2D RectMask2D;
    int previousContainerSize=-1;
    private RectTransform rectTransform;
    private RectTransform rectTransformParent;
    private float previousPixelDimensions;

    private void Start()
    {
        Setup();
    }
    private void Update()
    {
        if (ChangeInSettings())
        {
            Setup();
        }
    }

    private bool ChangeInSettings()
    {
        CheckProperties();        
        if(!CanRun())
            return false;

        bool ContainerHasChangedInSize = previousContainerSize != MenuContainer.GetContainerSize();
        bool PixelDimensionsChangedInSize = previousPixelDimensions != MenuContainer.GetPixelDimension();
        bool padDifference=previousPadding!=Padding;
        previousPixelDimensions=MenuContainer.GetPixelDimension();
        previousContainerSize=MenuContainer.GetContainerSize();
        previousPadding=Padding;
        return ContainerHasChangedInSize || PixelDimensionsChangedInSize || padDifference;
    }


    private void Setup()
    {
        CheckProperties();
        if(!CanRun())
            return;

        if(GetSize()==0)
            return;
        float SizeWithPadding = (GetSize() + (Padding));
        if (MenuContainer.GetOrientation() == Orientation.Vertical)
        {
            rectTransformParent.sizeDelta = new Vector2(rectTransformParent.sizeDelta.x, SizeWithPadding);
            rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, Padding / 2);
            rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, -Padding / 2);
        }
        else
        {
            rectTransformParent.sizeDelta = new Vector2(SizeWithPadding, rectTransformParent.sizeDelta.y);
            rectTransform.offsetMin = new Vector2(Padding/2,rectTransform.offsetMin.y );
            rectTransform.offsetMax = new Vector2(-Padding/2, rectTransform.offsetMax.y );
        }
    }

    private bool CanRun()
    {
        return MenuContainer!=null && rectTransform != null && rectTransformParent !=null;
    }

    private void CheckProperties()
    {
        if (MenuContainer == null)
        {
            MenuContainer = GetComponentInChildren<MenuControllerContainer>();
        }
        if (rectTransform == null)
        {
            SetupMenuWrapper();
        }
        if (rectTransformParent == null)
        {
            rectTransformParent = transform.parent.GetComponent<RectTransform>();
        }
    }

    private void SetupMenuWrapper()
    {
        rectTransform = GetComponent<RectTransform>();
        if (MenuContainer.GetOrientation() == Orientation.Vertical)
        {
            rectTransform.anchorMin = new Vector2(0.5f, 0f);
            rectTransform.anchorMax = new Vector2(0.5f, 1);
            rectTransform.pivot = new Vector2(0.5f, 1);
        }
        else
        {
            rectTransform.anchorMin = new Vector2(0f, 0.5f);
            rectTransform.anchorMax = new Vector2(1f, 0.5f);
            rectTransform.pivot = new Vector2(1f,0.5f);            
        }
    }

    private void SetupPanelWrapper()
    {
        rectTransformParent = GetComponent<RectTransform>();

        rectTransformParent.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransformParent.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransformParent.pivot = new Vector2(0.5f, 0.5f);
    }

    private float GetSize()
    {
        
        if(MenuContainer==null)
        {
            MenuContainer=GetComponentInChildren<MenuControllerContainer>();
        }

        int containersize=MenuContainer.GetContainerSize();
        float pixelDimensions=MenuContainer.GetPixelDimension();
        return (pixelDimensions*containersize);
    }
}
