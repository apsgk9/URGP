using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Interaction
{
    public interface IInteractable
    {
        //The GameObject this interactable is on
        string Name {get;}
        string Description {get;}
        GameObject gameObject {get;}
        public void Interact();
    }
}