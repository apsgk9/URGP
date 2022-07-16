using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityLogger : MonoBehaviour
{
    Rigidbody Rigidbody;
    private void Awake()
    {
        Rigidbody= GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        Debug.Log("Velocity: "+ Rigidbody.velocity);
    }
}
