using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableTest : MonoBehaviour, Interaction.InteractablePrompt
{
    public string Name => gameObject.name;

    public string Description => "TEST";

    public void Interact()
    {
        //Debug.Log("Interacted with "+gameObject.name);
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
