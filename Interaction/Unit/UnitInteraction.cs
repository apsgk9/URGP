using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitInteraction : MonoBehaviour
{
    public int UpdateRate=60;
    [SerializeField]
    private float _timeInterval;
    [Min(0)]
    public float _Distance=3f;
    public float Distance{get {return _Distance;} set{ _Distance= Mathf.Max(0,value);}}
    [SerializeField]

    public float CurrentTimeUpdate;

    private bool isFocused=false;
    [SerializeField]
    private float distanceFoundSquared;

    private void Start()
    {
        CurrentTimeUpdate=Time.time;
        _timeInterval=1.0f/((float)UpdateRate);
    }
    private void Update()
    {
        if(GameMode.IPlayerCentric.Player==null)
            return;
        if(Time.time-CurrentTimeUpdate>(_timeInterval))
        {
            //Debug.Log("UPDATE");
            CurrentTimeUpdate=Time.time;
            Vector3 deltaVectorFromPlayer = transform.position - GameMode.IPlayerCentric.Player.transform.position;
            distanceFoundSquared = deltaVectorFromPlayer.sqrMagnitude;

            if((Distance*Distance)>distanceFoundSquared)
            {
                WithinSphere();
            }
            else
            {
                OutsideSphere();
            }
            
            


        }
        
    }

    private void OutsideSphere()
    {
        if(!isFocused)
            return;
        CameraManager.Instance.PlayerCameraManagerReference.RemoveAdditionalFocus(this.transform);
        isFocused=false;
    }

    private void WithinSphere()
    {
        if(isFocused)
            return;
        CameraManager.Instance.PlayerCameraManagerReference.AddtionalFocus(this.transform);
        isFocused=true;
    }

    private void OnDrawGizmosSelected()
    {        
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, Distance);
    }
}
