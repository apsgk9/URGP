using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MainObject;

public abstract class CameraPlayer : MainObject<CameraPlayer> , IFocusedCamera
{    public int Priority= 10;

    [SerializeField] [ReadOnly]
    private Transform _mainTransformToFocus;
    public Transform MainTransformToFocus { get => _mainTransformToFocus; set => _mainTransformToFocus=value; }

    public List<Transform> AdditionalTransformsToFocus;

    virtual public void SetMainFocusTo(Transform target)
    {
        HandleNewMainFocusedTarget(target);
    }
    //Should be called when GameMode.IPlayerCentric.PlayerHasChangedTo has activated
    /*
        This is the only way the camera knows if the character has changed.
    */
    virtual protected void RequestToSwitchCameraToNewPlayer(GameObject previousPlayer,GameObject changedPlayerObject)
    {
        //Debug.Log(gameObject.name + " Base: PlayerHasChanged");
        // will not assign the main player if its not the main camera. However this can be called
        // before the new player is assigned. So it causes problems.

        if(!isMain)
        {            
            //Debug.Log("DENIED: "+ name);
            return;
        }
        //Debug.Log("GRANTED: "+ name);
        SetMainFocusTo(changedPlayerObject.transform);
    }


    virtual protected Transform HandleNewMainFocusedTarget(Transform t)
    {
        var Target = t.GetComponentInChildren<CameraTarget>();
        if (Target)
        {
            MainTransformToFocus = Target.transform;
        }
        else
        {
            MainTransformToFocus = t;
        }
        return MainTransformToFocus;
    }

    virtual public Transform GetCameraTransform()
    {
        var playercam=CameraManager.Instance.GetPlayerCamera();
        if(playercam==null)
            return transform;            
        return playercam.transform;
    }

    virtual public void HandleBeingMainCamPlayer()
    {
        if(!isMain)
        {
            Debug.LogError("This camera is supposed to be a main camera.");
            return;
        }
    }
    //Camera is not main Camera,
    virtual public void HandleBeingOffstageCamPlayer()
    {
        if(isMain)
        {
            Debug.LogError("This camera is not supposed to be a main camera.");
            return;
        }

    }
    virtual protected void OnEnable()
    {        
        MainObject<CameraPlayer>.NewMainObjectHasBeenSet += MainPlayerCameraHasChanged;
        GameMode.IPlayerCentric.SwitchCameraToNewPlayer += RequestToSwitchCameraToNewPlayer;
    }

    virtual protected void OnDisable()
    {
        MainObject<CameraPlayer>.NewMainObjectHasBeenSet -= MainPlayerCameraHasChanged;
        GameMode.IPlayerCentric.SwitchCameraToNewPlayer -= RequestToSwitchCameraToNewPlayer;
    }
   
   /*
        Gets called every single time the main cameraPlayer has changed
   */
    private void MainPlayerCameraHasChanged(MainObject<CameraPlayer> newMainCam)
    {
        if(newMainCam.GetComponent<CameraPlayer>() ==this) //The new camera might call its own invokation function
        {
            //Debug.Log("NEW MAIN CAMERA: "+newMainCam.gameObject.name);
            HandleBeingMainCamPlayer();
            return;
        }
        //Debug.Log("BECOME OFFSTAGE CAMERA: "+newMainCam.gameObject.name);
        HandleBeingOffstageCamPlayer();
    }
    
}
