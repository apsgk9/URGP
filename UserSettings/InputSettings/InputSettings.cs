using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "InputSettings", menuName = "ScriptableObjects/Settings/InputSettings", order = 1)]
public class InputSettings : ScriptableObject, Service.IGameService
{
    [Header("Controller")]
    public float ControllerYAxisSensitivity= 1500f;
    public float ControllerXAxisSensitivity= 1500f;

    public float ControllerZoomSensitivity = 30f;
    [Header("Mouse")]
    public float MouseXSensitivity = 1;
    public float MouseYSensitivity = 1;
    public float ScrollSensitivity = 75f;
}
