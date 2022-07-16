using System;
using Cinemachine;
using Cinemachine.Utility;
using Service;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CinemachineFreeLook))]
public class CinemachineFreeLookZoom : MonoBehaviour
{
    [Tooltip("The value of the input axis.  A value of 0 means no input.  You can drive "
        + "this directly from a custom input system.")]
    public float Value {get; private set;}
    private CinemachineFreeLook freelook;
    private CinemachineFreeLook.Orbit[] originalOrbits;
    [Tooltip("The minimum scale for the orbits")]
    [Range(0.01f, 1f)]
    public float minScale = 0.5f;

    [Tooltip("The maximum scale for the orbits")]
    [Range(1F, 5f)]
    public float maxScale = 1.5f;

    public float CurrentScale = 1f;

    [SaveDuringPlay]
    [Tooltip("How fast should zoom should be")]
    [Range(0.01f, 1f)]
    public float Multiplier = 1f;



    [SaveDuringPlay]
    [Tooltip("The amount of time in seconds it takes to accelerate to a higher speed")]
    public float accelTime;
    [SaveDuringPlay]

    [Tooltip("The amount of time in seconds it takes to decelerate to a lower speed")]
    public float decelTime;


    /// Internal state
    private float mCurrentSpeed;
    const float Epsilon = UnityVectorExtensions.Epsilon;
    void OnValidate()
    {
        minScale = Mathf.Max(0.01f, minScale);
        maxScale = Mathf.Max(minScale, maxScale);
        accelTime = Mathf.Max(0, accelTime);
        decelTime = Mathf.Max(0, decelTime);
    }
    void Awake()
    {
        freelook = GetComponentInChildren<CinemachineFreeLook>();
        if (freelook != null)
        {
            originalOrbits = new CinemachineFreeLook.Orbit[freelook.m_Orbits.Length];
            for (int i = 0; i < originalOrbits.Length; i++)
            {
                originalOrbits[i].m_Height = freelook.m_Orbits[i].m_Height;
                originalOrbits[i].m_Radius = freelook.m_Orbits[i].m_Radius;
            }
#if UNITY_EDITOR
            SaveDuringPlay.SaveDuringPlay.OnHotSave -= RestoreOriginalOrbits;
            SaveDuringPlay.SaveDuringPlay.OnHotSave += RestoreOriginalOrbits;
#endif
        }
    }

#if UNITY_EDITOR
    private void OnDestroy()
    {
        SaveDuringPlay.SaveDuringPlay.OnHotSave -= RestoreOriginalOrbits;
    }

    private void RestoreOriginalOrbits()
    {
        if (originalOrbits != null)
        {
            for (int i = 0; i < originalOrbits.Length; i++)
            {
                freelook.m_Orbits[i].m_Height = originalOrbits[i].m_Height;
                freelook.m_Orbits[i].m_Radius = originalOrbits[i].m_Radius;
            }
        }
    }
#endif
    
    void Update()
    {
        
        Zoom();
    }

    private void Zoom()
    {
        if (originalOrbits != null)
        {
            float deltaScale = DampenValue(Time.unscaledDeltaTime, Value);

            CurrentScale += deltaScale;
            CurrentScale = Mathf.Clamp(CurrentScale, minScale, maxScale);
            SetScale(CurrentScale);
        }
    }

    public void SetScale(float inputScale)
    {
        for (int i = 0; i < originalOrbits.Length; i++)
        {
            freelook.m_Orbits[i].m_Height = originalOrbits[i].m_Height * inputScale;
            freelook.m_Orbits[i].m_Radius = originalOrbits[i].m_Radius * inputScale;
        }
    }

    
    public void SetPercentScale(float percent)
    {
        float inputScale = ((maxScale-minScale)*percent)+minScale;
        SetScale(inputScale);
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