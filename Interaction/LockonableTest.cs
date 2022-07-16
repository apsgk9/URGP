using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interaction;
public class LockonableTest : MonoBehaviour , ILockables
{
    public string Name => gameObject.name;

    public string Description => "TEST LOCKON";

    public void Interact()
    {
        Debug.Log("LOCKON INTERACT");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
