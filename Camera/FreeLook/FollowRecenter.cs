using UnityEngine;
using Cinemachine;
using Cinemachine.Utility;
using System;
//https://forum.unity.com/threads/trouble-recentering-the-x-axis-of-the-free-look-camera.539097/
public class FollowRecenter : MonoBehaviour
{
    public static bool Recentering;
    public float recenterTime = 0.5f;
    CinemachineFreeLook FreeLookVirtualCam;
    public bool recenter;


    void Start()
    {
        FreeLookVirtualCam = GetComponent<CinemachineFreeLook>();
        FixTimeRecentering();
        FreeLookVirtualCam.m_RecenterToTargetHeading.m_enabled=Recentering;        
        FreeLookVirtualCam.m_YAxisRecentering.m_enabled=Recentering;
    }
    void Update()
    {
        Transform target = FreeLookVirtualCam != null ? FreeLookVirtualCam.Follow : null;
        if (target == null)
            return;
        RecenterCheck();
        Recenter();
    }

    private void Recenter()
    {        
        if(!Recentering)
        {
            FreeLookVirtualCam.m_RecenterToTargetHeading.CancelRecentering();
            FreeLookVirtualCam.m_YAxisRecentering.CancelRecentering();
        }
        FreeLookVirtualCam.m_RecenterToTargetHeading.m_enabled=Recentering;        
        FreeLookVirtualCam.m_YAxisRecentering.m_enabled=Recentering;
    }

    private bool isOnCenter()
    {
        Transform target = FreeLookVirtualCam.Follow;

        // How far away from centered are we?
        Vector3 up = FreeLookVirtualCam.State.ReferenceUp;
        Vector3 back = FreeLookVirtualCam.transform.position - target.position;
        float angle = UnityVectorExtensions.SignedAngle(
            back.ProjectOntoPlane(up), -target.forward.ProjectOntoPlane(up), up);

        if (Mathf.Abs(angle) < 0.2)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void RecenterCheck()
    {
        Recentering=UserInput.Instance.isPlayerLookIdle &&
        //!PlayerInput.Instance.isPlayerTryingToMove &&
        !isOnCenter();
        recenter=Recentering;
    }
    private void FixTimeRecentering()
    {
        FreeLookVirtualCam.m_RecenterToTargetHeading.m_RecenteringTime = recenterTime;
        FreeLookVirtualCam.m_YAxisRecentering.m_RecenteringTime = recenterTime;
        FreeLookVirtualCam.m_RecenterToTargetHeading.m_WaitTime = 0;
        FreeLookVirtualCam.m_YAxisRecentering.m_WaitTime = 0;
    }
    private void OnValidate()
    {
        FreeLookVirtualCam = GetComponent<CinemachineFreeLook>();
        FixTimeRecentering();        
    }
}