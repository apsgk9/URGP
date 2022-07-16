using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private CinemachineFreeLook freeLook;

    // Start is called before the first frame update
    private void Awake()
    {
        freeLook = GetComponent<CinemachineFreeLook>();        
    }
    // Update is called once per frame
    private void ShakeOn()
    {
        //freeLook.
    }
}
