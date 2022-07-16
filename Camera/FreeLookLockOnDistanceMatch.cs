using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class FreeLookLockOnDistanceMatch : MonoBehaviour
{
    public CinemachineFreeLook FreeLookVMCam;
    public CinemachineVirtualCamera LockOnVMCam;
    public CinemachineFreeLookZoom FreeLookZoom;
    public CinemachineCameraDistanceZoom LockOnZoom;
    void Start()
    {
        if(FreeLookVMCam==null || LockOnVMCam==null || FreeLookZoom ==null || LockOnZoom==null)
        {
            Debug.LogError("Properties have not been set.");
        }

        //Update Once rather update every frame. Causes hitches in some transitions if done so.
        //Still works out if doesn't
        MatchDistance();
    }
    
    private void MatchDistance()
    {
        if (FreeLookVMCam.enabled == false)
        {
            float percent = (LockOnZoom.CurrentDistance - LockOnZoom.minDistance) / (LockOnZoom.maxDistance - LockOnZoom.minDistance);
            FreeLookZoom.SetPercentScale(percent);
        }
        else if (LockOnVMCam.enabled == false)
        {
            float percent = (FreeLookZoom.CurrentScale - FreeLookZoom.minScale) / (FreeLookZoom.maxScale - FreeLookZoom.minScale);
            LockOnZoom.SetPercentDistance(percent);
        }
    }
}
