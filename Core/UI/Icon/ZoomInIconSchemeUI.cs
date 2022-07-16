using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class ZoomInIconSchemeUI : IconSchemeBaseUI
{
    [Header("Parent Gameobjects")]
    [SerializeField]
    private GameObject ControllerZoomParent;
    [SerializeField]
    private GameObject PCZoomParent;
    [Header("Images")]
    [SerializeField]
    
    private Image ZoomInIconController;
    [SerializeField]
    private Image ZoomOutIconController;
    [SerializeField]
    private Image ZoomIconPC;
    [Header("InputActionReference")]
    [SerializeField]
    private InputActionReference ZoomINControllerIA;
    [SerializeField]
    private InputActionReference ZoomOUTControllerIA;
    [SerializeField]
    private InputActionReference ZoomPCIA;
    
    protected override void ChangeInControllerType(UserControllerType NewType)
    {
        if(UserControllerType.PlayStation==NewType || UserControllerType.Xbox==NewType)
        {
            SwitchToController();
            //NOTE-- This is kinda confusing since zooming in and out is arbitrary due to the fact that it can be inversed
            ZoomInIconController.sprite=KeyPromptManager.Instance.GetIcon(IAUtil.InputActionsUtility.GetReadableTextFromInputActionReference(ZoomINControllerIA));
            ZoomOutIconController.sprite=KeyPromptManager.Instance.GetIcon(IAUtil.InputActionsUtility.GetReadableTextFromInputActionReference(ZoomOUTControllerIA));
        }
        else if(UserControllerType.PC==NewType)
        {
            SwitchToPC();
            ZoomIconPC.sprite=KeyPromptManager.Instance.GetIcon(IAUtil.InputActionsUtility.GetReadableTextFromInputActionReference(ZoomPCIA));
        }
        else
        {
            ZoomInIconController.sprite=KeyPromptManager.Instance.MiscPrompts.Unknown;
            ZoomOutIconController.sprite=KeyPromptManager.Instance.MiscPrompts.Unknown;
            ZoomIconPC.sprite=KeyPromptManager.Instance.MiscPrompts.Unknown;
        }
    }

    private void SwitchToPC()
    {
        //if(ControllerZoomParent.activeSelf)
            ControllerZoomParent.SetActive(false);
        //if(PCZoomParent.activeSelf)
            PCZoomParent.SetActive(true);
    }

    private void SwitchToController()
    {
        //if(ControllerZoomParent.activeSelf)
            ControllerZoomParent.SetActive(true);
        //if(PCZoomParent.activeInHierarchy)
            PCZoomParent.SetActive(false);
    }
}
