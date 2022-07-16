using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UI.Character
{
    public interface ICharacterUI
    {
        GameObject AssociatedCharacter{get;}
        void SetCharacter(GameObject CharacterObject);
    }
}