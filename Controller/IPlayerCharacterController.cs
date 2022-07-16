using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controller
{
    public interface IPlayerCharacterController : ICharacterController
    {        
        bool inControl {get; set;}
    }
}
