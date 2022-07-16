using System;
using System.Collections;
using System.Collections.Generic;
using Controller;
using Interaction;
using UnityEngine;

//There should only be one at a time? Have to check since there can be multiple players
public class InteractorPlayer : AngledInteractor<InteractablePrompt>
{
    private InteractionMenuHandler _InteractionMenuHandler;
    private List<(ButtonMenu,InteractablePrompt)> InteractableButtonCurrentlyInMenu;

    public IPlayerCharacterController PlayerCharacterControllerReference;

    private bool ApplicationQuitting;

    private void Awake()
    {
        InteractableButtonCurrentlyInMenu = new List<(ButtonMenu,InteractablePrompt)>();
        PlayerCharacterControllerReference = GetComponent<IPlayerCharacterController>();
    }
    private void OnDisable()
    {
        if(ApplicationQuitting)
            return;
        RemoveButtonsInMenu();
    }
    private void OnApplicationQuit()
    {
        ApplicationQuitting=true;
    }

    protected override void Update()
    {
        //Disable if this PlayerObject is not being used.
        bool isMainPlayer = PlayerCharacterControllerReference == null || !PlayerCharacterControllerReference.inControl;
        if (isMainPlayer || GameState.isPaused)
        {
            RemoveButtonsInMenu();
            return;
        }



        if(Service.ServiceLocator.Current==null || !Service.ServiceLocator.Current.Exists<InteractionMenuHandler>())
            return;
        if(Service.ServiceLocator.Current.Get<InteractionMenuHandler>() != _InteractionMenuHandler)
        {
            _InteractionMenuHandler= Service.ServiceLocator.Current.Get<InteractionMenuHandler>();
        }


        base.Update(); //Find the interacatables.


        List<(ButtonMenu,InteractablePrompt)> potentialButtonsToRemove= new List<(ButtonMenu,InteractablePrompt)>(InteractableButtonCurrentlyInMenu);
        List<ButtonMenu> NewMenu= new List<ButtonMenu>();

        foreach (InteractablePrompt InteractableOnFieldItem in InteractablesOnFieldList)
        {
            bool interactableIsInMenu=false;
            ButtonMenu buttonFoundInMenu=null;
            //Compare with the other List. Wittle Down this until there is no more
            foreach ((ButtonMenu,InteractablePrompt) ptnBtn in potentialButtonsToRemove)
            {
                if(InteractableOnFieldItem!=ptnBtn.Item2)
                {
                    continue;
                }
                interactableIsInMenu=true;
                buttonFoundInMenu=ptnBtn.Item1;
                break;
            }
            if(!interactableIsInMenu)
            {
                ButtonMenu newBtn = _InteractionMenuHandler.CreateButton();
                (ButtonMenu, InteractablePrompt) toAdd = (newBtn, InteractableOnFieldItem);

                InteractableButtonCurrentlyInMenu.Add(toAdd);
                newBtn.OnButtonConfirm+= InteractableOnFieldItem.Interact;

                ButtonMainText buttonMainText = newBtn.GetComponentInChildren<ButtonMainText>();

                if(buttonMainText!=null && buttonMainText.TMPText!=null)
                {
                    buttonMainText.TMPText.text= InteractableOnFieldItem.Name;
                }
                

            }
            if (buttonFoundInMenu!=null)
            {
                (ButtonMenu, InteractablePrompt) toRemove = (buttonFoundInMenu, InteractableOnFieldItem);
                potentialButtonsToRemove.Remove(toRemove);
                
            }
        }
        
        foreach ((ButtonMenu,InteractablePrompt) btnTuple in potentialButtonsToRemove)
        {
            btnTuple.Item1.OnButtonConfirm-= btnTuple.Item2.Interact;
            _InteractionMenuHandler.RemoveButton(btnTuple.Item1);
            InteractableButtonCurrentlyInMenu.Remove(btnTuple);            
        }

        
    }

    private void RemoveButtonsInMenu()
    {   
        if(InteractableButtonCurrentlyInMenu!=null && InteractableButtonCurrentlyInMenu.Count==0)
            return;
        
        List<(ButtonMenu, InteractablePrompt)> ButtonsToRemove = new List<(ButtonMenu, InteractablePrompt)>(InteractableButtonCurrentlyInMenu);
        foreach ((ButtonMenu, InteractablePrompt) btnTuple in ButtonsToRemove)
        {
            btnTuple.Item1.OnButtonConfirm -= btnTuple.Item2.Interact;
            _InteractionMenuHandler.RemoveButton(btnTuple.Item1);
            InteractableButtonCurrentlyInMenu.Remove(btnTuple);
        }
    }
}
