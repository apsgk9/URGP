using System;

namespace CharacterProperties
{
    public interface ICharacterModifierStamina
    {
        bool IsStaminaBeingUsed{get;set;}
        
        bool CanUse();
        
        //current,min,max
        event Action<float,float,float,bool> OnStaminaChanged;
        float ChangeStamina(float newStamina,bool shouldWaitForRegen=true);
        float AddStamina(float changeToAdd,bool shouldWaitForRegen=true);
        float GetCurrentStamina();
        float GetMaxStamina();
        float GetMinStamina();
    }
}
