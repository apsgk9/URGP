using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MainObject;

/*
    Please Fix Since No new setup
*/
public class UIPlayer : MonoBehaviour, ICameraUser, IPlayerUser
{
    public GameObject FocusedPlayer;
    public Camera Camera;
    List<ICameraUser> CameraUsers;
    List<IPlayerUser> PlayerUsers;

  //Need to Replace this so that UI should change when the player has changed. But should work with all
  //Generic Centric Game Modes

  
    private void OnEnable()
    {
        GameMode.IPlayerCentric.PlayerHasChangedTo+=MainPlayerObjectHasChanged;
        
    }
    private void OnDisable()
    {
        GameMode.IPlayerCentric.PlayerHasChangedTo-=MainPlayerObjectHasChanged;
    }
    private void MainPlayerObjectHasChanged(GameObject PreviousPlayer, GameObject PlayerObject)
    {
        FocusedPlayer = PlayerObject;
        if (PlayerUsers == null)
        {
            PlayerUsers = GetComponentsInChildren<IPlayerUser>().ToList();
        }
        SetPlayerUsers();
    }

    public void Setup()
    {
        FindAllUsers();
        SetUsers();
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
    }

    public void FindAllUsers()
    {
        CameraUsers = GetComponentsInChildren<ICameraUser>().ToList();
        PlayerUsers = GetComponentsInChildren<IPlayerUser>().ToList();
    }

    public void SetUsers()
    {
        SetCamUsers();
        SetPlayerUsers();
    }

    private void SetPlayerUsers()
    {
        if (PlayerUsers == null)
        {
            Debug.Log("No Player Users.");
            return;
        }
        foreach (var playerUser in PlayerUsers)
        {
            playerUser.SetPlayer(FocusedPlayer);
        }
    }

    private void SetCamUsers()
    {
        if (PlayerUsers == null)
        {
            Debug.Log("No Cam Users.");
            return;
        }
        foreach (var camUser in CameraUsers)
        {
            camUser.SetCamera(Camera);
        }
    }

    public void SetCamera(Camera cam)
    {
        Camera = cam;
    }

    public void SetPlayer(GameObject playerObject)
    {
        FocusedPlayer = playerObject;
    }
}
