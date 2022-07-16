using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Cinemachine;
using Cinemachine.Utility;

public class CinemachineCameraDistanceZoom : MonoBehaviour
{
    [Tooltip("The value of the input axis.  A value of 0 means no input.  You can drive "
        + "this directly from a custom input system.")]
    public float Value {get; private set;}
    private CinemachineFramingTransposer CinemachineFramingTransposer;
    [Tooltip("The minimum scale for the orbits")]
    //[Range(0.00f, 1f)]
    [Min(0)]
    public float minDistance = 0f;

    [Tooltip("The maximum scale for the orbits")]
    //[Range(1F, 5f)]
    public float maxDistance = 10f;

    public float CurrentDistance = 3f;

    [SaveDuringPlay]
    [Tooltip("How fast should zoom should be")]
    [Range(0.01f, 2f)]
    public float Multiplier = 1f;



    [SaveDuringPlay]
    [Tooltip("The amount of time in seconds it takes to accelerate to a higher speed")]
    public float accelTime=0.25f;
    [SaveDuringPlay]

    [Tooltip("The amount of time in seconds it takes to decelerate to a lower speed")]
    public float decelTime=0.75f;
    [SerializeField]
    private CinemachineVirtualCamera vcam;


    /// Internal state
    private float mCurrentSpeed;
    const float Epsilon = UnityVectorExtensions.Epsilon;
    void OnValidate()
    {
        //minDistance = Mathf.Max(0.00f, minDistance);
        maxDistance = Mathf.Max(minDistance, maxDistance);
        accelTime = Mathf.Max(0, accelTime);
        decelTime = Mathf.Max(0, decelTime);
        CurrentDistance = Mathf.Clamp(CurrentDistance, minDistance, maxDistance);

        vcam = GetComponent<CinemachineVirtualCamera>();
        CinemachineFramingTransposer = vcam.GetCinemachineComponent<CinemachineFramingTransposer>();
        if(CinemachineFramingTransposer)
        {
            CinemachineFramingTransposer.m_CameraDistance=CurrentDistance;
        }
    }
    void Awake()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
        CinemachineFramingTransposer = vcam.GetCinemachineComponent<CinemachineFramingTransposer>();
    }
    void Update()
    {        
        Zoom();
    }

    private void Zoom()
    {
        float deltaScale = DampenValue(Time.unscaledDeltaTime, Value);

        CurrentDistance += deltaScale;
        CurrentDistance = Mathf.Clamp(CurrentDistance, minDistance, maxDistance);
        SetDistance(CurrentDistance);

    }

    public void SetDistance(float inputDistance)
    {
        CinemachineFramingTransposer.m_CameraDistance = inputDistance;
    }

    public void SetPercentDistance(float percent)
    {
        SetDistance(((maxDistance-minDistance)*percent)+minDistance);
    }

    public void UpdateScrollValue(float input)
    {
        bool OverUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        bool InteractionMenuOpen = Service.ServiceLocator.Current.Exists<InteractionMenuHandler>() && Service.ServiceLocator.Current.Get<InteractionMenuHandler>().Active;
        if (OverUI || InteractionMenuOpen)
        {            
            Value=0;
        }
        else
        {
            Value=input;
        }
    }    

    private float DampenValue(float deltaTime, float inputValue)
    {
        float input = inputValue * Multiplier / 10f;
        if (deltaTime < Epsilon)
        {
            mCurrentSpeed = 0;
        }
        else
        {
            float speed = input / deltaTime;
            float dampTime = Mathf.Abs(speed) < Mathf.Abs(mCurrentSpeed) ? decelTime : accelTime;
            speed = mCurrentSpeed + Damper.Damp(speed - mCurrentSpeed, dampTime, deltaTime);
            mCurrentSpeed = speed;
            input = speed * deltaTime;
        }
        return input;
    }
}
