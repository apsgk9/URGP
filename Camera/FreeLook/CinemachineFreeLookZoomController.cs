  
using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineFreeLookZoom))]
public class CinemachineFreeLookZoomController : MonoBehaviour
{
    private CinemachineFreeLookZoom freelookZoom;
    private InputSettings _InputSettings;

    [SerializeField]
    private float ControllerValue;
    [SerializeField]
    private float DesktopValue;

    private void Awake()
    {
        freelookZoom = GetComponentInChildren<CinemachineFreeLookZoom>();        
    }

    //For some reason this is bugging out in build, best to disable it for now
    private IEnumerator Start()
    {
        
        while(Service.ServiceLocator.Current == null)
            yield return null;
        while(!Service.ServiceLocator.Current.Exists<SettingsManager>())
            yield return null;
        _InputSettings= Service.ServiceLocator.Current.Get<SettingsManager>().GetInputSettings();
    }



    // Update is called once per frame
    void Update()
    {
        //freelookZoom.Value=0f;
        freelookZoom.UpdateScrollValue(0);
        if(!GameState.isPaused)
        {
            UpdateDesktopValues();
            SetZoomValue(DesktopValue+ControllerValue);
            ControllerValue=0;
            DesktopValue=0;
        }
    }

    private void UpdateDesktopValues()
    {
        DesktopValue=UserInput.Instance.Scroll*_InputSettings.ScrollSensitivity*Time.unscaledDeltaTime;
    }
    public void SetZoomValue(float value)
    {
        //freelookZoom.Value+=value;
        freelookZoom.UpdateScrollValue(value);
    }

    public void SetZoomValueWithControllerSensititivity(float value)
    {
        ControllerValue=value*_InputSettings.ControllerZoomSensitivity*Time.unscaledDeltaTime;
    }
}