using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnWhenCollided : MonoBehaviour
{    
    private void OnTriggerEnter(Collider other)
    {
        var pObject=other.GetComponent<Controller.IPlayerCharacterController>();
        if(pObject!=null)
        {
            SpawnPosition.SpawnAtAnyPosition(other.GetComponent<Transform>());
        }
        
    }
}
