using System;
using System.Collections;
using System.Collections.Generic;
using MainObject;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using UnitSystem;

/*
For reason it breaks with normal test mover
*/
public class PlayerPartySwitcher : MonoBehaviour
{
    [Header("Player")]
    [SerializeField]
    private GameObject MainUnit;

    public List<GameObject> Party= new List<GameObject>();

    public List<GameObject> DestroyedObjectsInParty;

    private bool NewMainUnitUpdate;
    public bool SwitchLeftUnit;
    public bool SwitchRightUnit;
    private bool finishedStart=false;

    [Min(0)] [SerializeField]
    private float Cooldown=0.1f;
    private float _cooldownUsedAtTime;

    public int SelectedIndex=0;

    private void Awake()
    {
        _cooldownUsedAtTime=Time.unscaledDeltaTime;
    }

    /*
        Maybe have it so it doesn't need to start this way every single time
    */
    private IEnumerator Start()
    {
        while(MainUnit==null)
        {
            MainUnit = GameMode.IPlayerCentric.Player;
            yield return null;
        }
        TryAddToPartyFront(MainUnit);

        finishedStart=true;
    }

    private void Update()
    {
        if(!finishedStart)
            return;
        if(GameState.isPaused)
            return;

       HandlePartySwitching();
    }

    private void OnEnable()
    {        
        RegisterEvents();
    }
    private void OnDisable()
    {
        DeregisterEvents();
    }

    private void RegisterEvents()
    {
        UserInput.Instance.PlayerInputActions.PlayerControls.ChangeCharacterLeft.started += HandleChangeCharacterLeftDown;
        UserInput.Instance.PlayerInputActions.PlayerControls.ChangeCharacterRight.started += HandleChangeCharacterRightDown;
        UnitSystem.GameUnit.PlayerGroupingChange+= ChangeInPlayerGroup;
    }

    private void DeregisterEvents()
    {
        if(UserInput.CanAccess)
        {
            UserInput.Instance.PlayerInputActions.PlayerControls.ChangeCharacterLeft.started -= HandleChangeCharacterLeftDown;
            UserInput.Instance.PlayerInputActions.PlayerControls.ChangeCharacterRight.started -= HandleChangeCharacterRightDown;
            UnitSystem.GameUnit.PlayerGroupingChange-= ChangeInPlayerGroup;
        }
    }

    private void ChangeInPlayerGroup(object sender, GameUnit.UnitArgs Args)
    {
        if(Args.unitParameters.Group == UnitSystem.UnitParameters.PLAYER_TAG)
        {
            TryAddToParty(Args.unit.gameObject);
        }
        else if(Args.unitParameters.Group == UnitSystem.UnitParameters.EMPTY_TAG && 
        Args.unitParameters.PreviousGroup == UnitSystem.UnitParameters.PLAYER_TAG)
        {
            bool result=TryRemoveFromParty(Args.unit.gameObject);
        }
        
    }

    

    private void HandleChangeCharacterRightDown(InputAction.CallbackContext obj)
    {
        if(GameState.isPaused)
            return;       
        if(SwitchLeftUnit==false && AbleToSwitch() && SwitchRightUnit==false)
        {
            SwitchLeftUnit=true;
        }
    }
    

    private void HandleChangeCharacterLeftDown(InputAction.CallbackContext obj)
    {
        if(GameState.isPaused)
            return;
        if(SwitchRightUnit==false && AbleToSwitch() && SwitchLeftUnit==false)
        {
            SwitchRightUnit=true;
        }
    }

    //TheMainUnit will always be in the party thus cannot be removed
    public bool CanBeRemoved(GameObject playerObject)
    {
        return MainUnit!=playerObject;
    }
    private bool TryRemoveFromParty(GameObject playerObject)
    {
        if(CanBeRemoved(playerObject))
        {
            if(!Party.Contains(playerObject))
                return false;
            Party.Remove(playerObject);
            return true;
        }
        return false;
    }
    public bool CanBeAddedToParty(GameObject playerObject)
    {
        return !Party.Contains(playerObject);
    }
    public bool TryAddToPartyFront(GameObject playerObject)
    {
        if(CanBeAddedToParty(playerObject))
        {
            Party.Insert(0,playerObject);
            return true;
        }
        return false;
    }

    public bool TryAddToParty(GameObject playerObject)
    {
        if(CanBeAddedToParty(playerObject))
        {
            Party.Add(playerObject);
            return true;
        }
        return false;
    }

    
    private void HandlePartySwitching()
    {
        if(SwitchLeftUnit || SwitchRightUnit) //currently the same right now
        {
            if(Time.time>_cooldownUsedAtTime+Cooldown)
            {
                _cooldownUsedAtTime=Time.unscaledDeltaTime;
            }
            else
                return;
            
            //Clean Party First. Must have no null values
             Party.RemoveAll(x => x == null);


            var OldMainUnit = MainUnit;
            var OldParty = Party;

            var NewMainUnit=GetNewMainUnitToSwitchTo();

            bool sucesss = PartySystem.CharacterSwitcher.SwitchUnit(OldMainUnit,NewMainUnit);

            if (!sucesss)
            {
                MainUnit = OldMainUnit;
                Party = OldParty;
            }
            else
            {
                MainUnit=NewMainUnit;
            }

            SwitchLeftUnit = false;
            SwitchRightUnit = false;
        }
    }


    //Figures which new Unit the player should switch to.
    public GameObject GetNewMainUnitToSwitchTo()
    {
        if (Party.Count == 0)
            return null;
        
        
        if(SwitchLeftUnit==true)
        {
            SelectedIndex= (SelectedIndex==0)? Party.Count-1:SelectedIndex-1;
        }
        else if(SwitchRightUnit==true)
        {
            SelectedIndex= (SelectedIndex==Party.Count-1)? 0: SelectedIndex+1;
        }
        else
        {
            return null;
        }

        return Party[SelectedIndex];
    }

    


    private bool AbleToSwitch()
    {        

        if(Party.Count==1)
        {

            var firstInLine= Party[0];

            if(firstInLine==MainUnit)
            {
                Party.RemoveAt(0);
                return false;
            }
        }        

        return Party.Count>0;
    }

}
