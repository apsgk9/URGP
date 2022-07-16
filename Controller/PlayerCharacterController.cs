using System;
using System.Collections;
using System.Collections.Generic;
using MainObject;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controller
{
    public class PlayerCharacterController : MonoBehaviour, IPlayerCharacterController
    {
        public bool inControl { get ; set; }
        public bool _previousInControl=false;
        [SerializeField]
        private Character Character;
        [SerializeField]
        private CharacterAction CharacterAction;
        private Transform _previousViewTransform;
        private bool _previousIsRunning;
        public bool Action_A=false;


        private void Awake()
        {
            _previousInControl = inControl;
            if(Character==null)
            {
                Character = GetComponent<Character>();
                if(Character!=null && Character is CharacterAction)
                {
                    CharacterAction=Character as CharacterAction;
                }
            }
            else
            {
                if(Character is CharacterAction)
                {
                    CharacterAction=Character as CharacterAction;                    
                }             
            }
        }
        private void OnEnable()
        {
            inControl=CanRun();
            _previousInControl = inControl;
            if(inControl)
            {
                RegisterActionEvents();
            }
            
            
            if (CharacterAction)
            {
                CharacterAction.OnGroundedAction += ResetAttack;
            }

        }        

        private void OnDisable()
        {
            DisableCharacterInputs();
        }

        private void DisableCharacterInputs()
        {
            DeregisterActionEvents();

            if (Character)
            {
                Character.HaltCharacterController();
            }
            if (CharacterAction)
            {
                CharacterAction.OnGroundedAction -= ResetAttack;
            }
        }

        private void Update()
        {
            HandleChangeInControl();

            if (CanRun() && Character)
            {
                SetInputToCharacterController();
            }

            
        }

        private void HandleChangeInControl()
        {
            if (_previousInControl != inControl)
            {
                if (inControl)
                {
                    RegisterActionEvents();
                }
                else
                {
                    DeregisterActionEvents();
                    
                    Character.HaltCharacterController();
                }
            }
            _previousInControl = inControl;
        }

        private void SetInputToCharacterController()
        {
            Character.IsThereMovement = IsThereMovement();
            Character.AttemptToJump = AttemptingToJump();
            Character.IsRunPressed = IsRunning();
            Character.ViewTransform = ViewTransform();
            Character.MovementVertical = MovementVertical();
            Character.MovementHorizontal = MovementHorizontal();
            if (CharacterAction)
            {
                CharacterAction.TriggerAction = GetAction_A();
            }
        }

        private bool GetAction_A()
        {
            return CanRun() ? Action_A : false;
        }

        private void HandleActionStart(InputAction.CallbackContext obj)
        {
            Action_A=true;
        }
        private void HandleActionEnd(InputAction.CallbackContext obj)
        {
            ResetAttack();
        }

        

        //private void ChangeInPlayer(MainObject<PlayerObject> obj)
        //{
        //    UserInput.Instance.ResetJump(); //Prevents the other new character from jumping if the jump button is still pressed
        //    //Should Probably zero out inputs here as well
        //}

        
        private bool CanRun()
        {
            return inControl;
        }


        public float MovementHorizontal()
        {
            return CanRun() ? UserInput.Instance.Horizontal : 0;
        }

        
        public float MovementVertical()
        {
            return CanRun()? UserInput.Instance.Vertical : 0;
        }
        public bool IsRunning()
        {
            return CanRun()? UserInput.Instance.RunPressed : false;
        }
        public bool IsThereMovement()
        {
            return CanRun()? UserInput.Instance.IsThereMovement(): false;
        }
        public bool AttemptingToJump()
        {
             return CanRun()? UserInput.Instance.JumpPressed: false;
        }

        public bool Attack()
        {
            return  CanRun() ? Action_A : false;
        }

        public void ResetAttack()
        {
            //At some point additional actions might be added so adding this here so I don't forget
            if(Action_A==false) 
                return;

            Action_A=false;
        }

        public Transform ViewTransform()
        {
            //if(Service.ServiceLocator.Current.Exists<PlayerCameraManager>())
            //{
            //    return Service.ServiceLocator.Current.Get<PlayerCameraManager>().GetPlayerCamera().transform;
            //}
            Camera playerCam = CameraManager.Instance.GetPlayerCamera();
            if (playerCam!=null)
            {
                return playerCam.transform;
            }
            return transform;
        }

        private void RegisterActionEvents()
        {
            if (UserInput.CanAccess)
            {
                UserInput.Instance.PlayerInputActions.PlayerControls.Action_A.started += HandleActionStart;
                UserInput.Instance.PlayerInputActions.PlayerControls.Action_A.canceled += HandleActionEnd;
            }
        }

        private void DeregisterActionEvents()
        {
            if (UserInput.CanAccess)
            {
                UserInput.Instance.PlayerInputActions.PlayerControls.Action_A.started -= HandleActionStart;
                UserInput.Instance.PlayerInputActions.PlayerControls.Action_A.canceled -= HandleActionEnd;
            }
        }
    }
}
