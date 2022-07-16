using UnityEngine;
using CharacterProperties;
using System;
/*
    Add's stamina behavior to a Characterstatemachine. Without it, the character can sprint infinitely.
    Use this to handle how much a character should sprint.
*/
public class CharacterStaminaModifier : CharacterModifier, ICharacterModifierStamina, IJumpUser
{
    [Header("Stamina")]
    public float CurrentStamina = 100;
    public int MaxStamina = 100;
    public int MinStaminaUsage = 20;
    public int MinStamina = 0;
    public bool ShouldCapMaxStamina = true;
    public bool ShouldCapMinStamina = true;

    [Header("Rate")]
    public uint UseRate = 10;
    public uint RegenRate = 5;
    public uint JumpUsage = 10;
    private bool _isStaminaBeingUsed = false;
    public bool IsStaminaBeingUsed { get => _isStaminaBeingUsed; set => _isStaminaBeingUsed = value; }
    public bool HasDrained { get; private set; }



    [Tooltip("Wait time before stamina regen.")]
    public uint StaminaRechargeDelay = 2;
    private bool SprintLock = false;
    private Timer StaminaRechargeTimer;

    public event Action<float, float, float,bool> OnStaminaChanged;

    private void Start()
    {
        StaminaRechargeTimer = new Timer(StaminaRechargeDelay);
        StaminaRechargeTimer.FinishTimer();
    }
   

    //Try to call only if there is change.
    //Stamina Timer Gets Reset if shouldWaitForRegenisActive
    public float AddStamina(float changeToAdd, bool shouldWaitForRegen = true)
    {
        CurrentStamina = ChangeStamina(CurrentStamina + changeToAdd, shouldWaitForRegen);
        return CurrentStamina;
    }

    public float ChangeStamina(float newStamina, bool shouldWaitForRegen = true)
    {
        CurrentStamina = newStamina;
        HandleStaminaHasRecovered();
        CapStamina();
        OnStaminaChanged?.Invoke(CurrentStamina, MinStamina, MaxStamina,false);

        if (shouldWaitForRegen == true)
        {
            StaminaRechargeTimer.ResetTimer();
        }
        return CurrentStamina;
    }

    

    private void HandleStaminaHasRecovered()
    {
        if (HasDrained == true)
        {
            if (CurrentStamina >= MinStaminaUsage)
            {
                HasDrained = false;
            }
        }
    }

    //Handle when stamina reaches the Max or Min limit.

    private void CapStamina()
    {
        if (ShouldCapMaxStamina && CurrentStamina > MaxStamina)
        {
            CurrentStamina = MaxStamina;
        }
        else if (ShouldCapMinStamina && CurrentStamina < MinStamina)
        {
            CurrentStamina = MinStamina;
            HasDrained=true;
        }
    }



    private void Update()
    {
        RegenStamina();
    }

    private void RegenStamina()
    {
        if (CurrentStamina < MaxStamina)
        {
            if (StaminaRechargeTimer.Activated)
            {
                AddStamina(RegenRate * Time.deltaTime, false);
            }
            else
            {
                StaminaRechargeTimer.Tick();
                //To prevent timer ui from fading out
                OnStaminaChanged?.Invoke(CurrentStamina, MinStamina, MaxStamina,true);
            }
        }
    }
    public bool CanUse()
    {
        return !HasDrained;
    }

    //Character uses on this method whenever sprint related behaviour is concerned
    public override void Handle(Character ctx)
    {
        if(ctx.IsJumpPressed)
        _isStaminaBeingUsed = false;
        //--Not Sprinting--- //Is Airborne or Not Sprinting
        if (ctx.AirborneMode != LocomotionEnmus.AirborneMode.Grounded || ctx.LocomotionMode != LocomotionEnmus.LocomotionMode.Sprint)
        {            
            if (CurrentStamina>MinStaminaUsage)
            {
                SprintLock=false;
            }
            return;
        }

        //--Character is Sprinting--
        if (HasDrained == false && SprintLock==false)
        {
            Sprint(ctx);
            return;
        }

        //Character Cannot Sprint so prevent it
        ctx.LocomotionMode = LocomotionEnmus.LocomotionMode.Run;
    }


    private void Sprint(Character ctx)
    {
        if(!isActive)
            return;

        _isStaminaBeingUsed = true;
        AddStamina(-UseRate * Time.deltaTime, true);
        ctx.LocomotionMode = LocomotionEnmus.LocomotionMode.Sprint;

        //Trigger if stamina is all used up
        if(HasDrained)
        {
            SprintLock = true;
        }
    }

    public void HandleJump(Character ctx)
    {
        if(!isActive)
            return;
        if(ctx.LocomotionMode == LocomotionEnmus.LocomotionMode.Sprint)
        {            
            AddStamina(-JumpUsage, true);
        }
    }
    
    //Might be a bit weird if you want stamina that doesn't go from min to max
    public float GetMaxStamina()
    {
        return MaxStamina;
    }

    public float GetMinStamina()
    {
        return MinStamina;
    }
    
    public float GetCurrentStamina()
    {
        return CurrentStamina;
    }
}
