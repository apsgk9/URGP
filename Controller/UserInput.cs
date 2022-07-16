using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/*
    It is best to call this instead of making your own Input Action.
    If you make multiple PlayerInputActions and each have its own events. It will lag.
    Here we have Inputs mostly used for the character controller.
*/

public class UserInput : Singleton<UserInput>, IUserInput
{
    public Vector2 DirectionVector => new Vector2(Horizontal, Vertical);
    [SerializeField] [ReadOnly]
    public Vector2 LastDirectionVector;
    public event Action<int> HotKeyPressed;
    public Vector2 CursorPosition => _mousePosition;
    public Vector2 ControllerCursorDeltaPosition => _analogAimPosition;
    public Vector2 MouseCursorDeltaPosition => _cursorDeltaPosition;
    public Vector2 RawControllerAim { get; private set; }
    [SerializeField] [ReadOnly]
    public Vector2 LastCursorPosition;
    public float IdleThreshold = 2f;
    public bool isPlayerLookIdle => MouseIdleTimer.Activated;
    private Timer MouseIdleTimer;
    public float Vertical => _vertical;
    public float Horizontal => _horizontal;
    private Vector2 _cursorDeltaPosition;
    private Vector2 _RawcursorDeltaPosition;
    [SerializeField] [ReadOnly]
    private Vector2 _mousePosition;
    [SerializeField] [ReadOnly]
    private Vector2 _analogAimPosition;
    [SerializeField] [ReadOnly]
    private float _vertical;
    [SerializeField] [ReadOnly]
    private float _horizontal;
    public bool RunPressed => _runPressed;
    [SerializeField] [ReadOnly]
    private bool _runPressed;
    public bool JumpPressed => _jumpPressed;
    [SerializeField] [ReadOnly]
    private bool _jumpPressed;

    
    private float _scroll;
    public float Scroll => _scroll;

    //New actions    
    public PlayerInputActions PlayerInputActions {get; private set;}
    private const int historyMaxLength = 4;

    public string DeviceUsing => _deviceUsing;


    private string _deviceUsing;
    private MovementHistory _verticalHistory;
    private MovementHistory _horizontalHistory;

    #region  InputSettings
    //bit iffy please consider somethings else.
    private InputSettings _InputSettings 
    {
        get 
        {
            if(_InputSettingsInstance==null)
            {
                if(Service.ServiceLocator.Current==null)
                    return null;
                if(!Service.ServiceLocator.Current.Exists<SettingsManager>())
                {
                    //Debug.LogWarning("Using TempSettings");
                    return _tempSettings;
                }
                var sManager =Service.ServiceLocator.Current.Get<SettingsManager>();
                if(sManager!=null && sManager.GetInputSettings()!=null)
                {
                    _InputSettingsInstance= sManager.GetInputSettings();
                }
                else
                {
                    return null;
                }
            }
            return _InputSettingsInstance;
        }
    }

    private InputSettings _InputSettingsInstance;
    private InputSettings _tempSettings;

    //Controller Type

    public Action<UserControllerType> CurrentControllerTypeHasChange;
    private UserControllerType _currentUserControllerType;
    public UserControllerType CurrentUserControllerType
    {
        get{ return _currentUserControllerType;}
        set
        {
            if(value!=_currentUserControllerType)
            {
                _currentUserControllerType=value;
                CurrentControllerTypeHasChange?.Invoke(value);
            }
        }
    }


    //Constant Strings
    private const string MOUSENAME= "Mouse";
    private const string KEYBOARDNAME= "Keyboard";
    private const string InitGameStateMachinePath = "Core/GameStateMachine";


    #endregion
    //---------------------------
    private void Awake()
    {
        CurrentUserControllerType=UserControllerType.Other;

        //For when settings are not found
        _tempSettings=ScriptableObject.CreateInstance<InputSettings>();

        //Instance = this;
        MouseIdleTimer = new Timer(IdleThreshold);
        LastDirectionVector = DirectionVector;

        PlayerInputActions = new PlayerInputActions();
        _deviceUsing = "Keyboard"; //default to keyboard

        //MovementAxisHistory
        _verticalHistory = new MovementHistory(historyMaxLength);
        _horizontalHistory = new MovementHistory(historyMaxLength);
    }
    private void OnEnable()
    {
        PlayerInputActions.Enable();
        //PlayerInputActions.PlayerControls.MovementAxis.performed += HandleMovement;
        //PlayerInputActions.PlayerControls.MovementAxis.canceled += ctx => HandleMovementCancel();

        PlayerInputActions.PlayerControls.MouseAim.performed += HandleMouseAim;
        PlayerInputActions.PlayerControls.MouseDeltaAim.performed += HandleMouseDeltaAim;
        PlayerInputActions.PlayerControls.MouseDeltaAim.canceled += ctx => _cursorDeltaPosition = Vector2.zero;

        PlayerInputActions.PlayerControls.AnalogAim.performed += HandleAnalogAim;
        PlayerInputActions.PlayerControls.Run.started += HandleRunPressed;
        PlayerInputActions.PlayerControls.Run.canceled += HandleRunReleased;


        PlayerInputActions.PlayerControls.Jump.started += HandleJumpStart;
        PlayerInputActions.PlayerControls.Jump.canceled += HandleJumpEnd;


        PlayerInputActions.PlayerControls.Scroll.started += HandleStartScroll;
        PlayerInputActions.PlayerControls.Scroll.canceled += HandleEndScroll;        

        //InputUser.onChange += OnDeviceChanged;
        //InputSystem.onDeviceChange+=InputDeviceChanged;
        InputSystem.onActionChange += OnActionChange;

        

    }

    

    private void OnDisable()
    {
        PlayerInputActions.Disable();
        //PlayerInputActions.PlayerControls.MovementAxis.performed -= HandleMovement;
        //PlayerInputActions.PlayerControls.MovementAxis.canceled -= ctx => HandleMovementCancel();

        PlayerInputActions.PlayerControls.MouseAim.performed -= HandleMouseAim;
        PlayerInputActions.PlayerControls.MouseDeltaAim.performed -= HandleMouseDeltaAim;
        PlayerInputActions.PlayerControls.AnalogAim.performed -= HandleAnalogAim;
        PlayerInputActions.PlayerControls.MouseDeltaAim.canceled -= ctx => _cursorDeltaPosition = Vector2.zero;
        PlayerInputActions.PlayerControls.Run.started += HandleRunPressed;
        PlayerInputActions.PlayerControls.Run.canceled += HandleRunReleased;

        PlayerInputActions.PlayerControls.Jump.started -= HandleJumpStart;
        PlayerInputActions.PlayerControls.Jump.canceled -= HandleJumpEnd;

        PlayerInputActions.PlayerControls.Scroll.started -= HandleStartScroll;
        PlayerInputActions.PlayerControls.Scroll.canceled -= HandleEndScroll;

        //InputUser.onChange -= OnDeviceChanged;  
        //InputSystem.onDeviceChange-=InputDeviceChanged;
        InputSystem.onActionChange += OnActionChange;


    }

    






    #region Tick
    private void Update()
    {
        //NonPaused Inputs
        RunNonTimeSensitiveInputs();
        if(GameState.isPaused==false)
        {            
            Tick();
        }
    }

    private void RunNonTimeSensitiveInputs()
    {
        var movementDirection= PlayerInputActions.PlayerControls.MovementAxis.ReadValue<Vector2>();
        _horizontal = movementDirection.x;
        _vertical = movementDirection.y;
    }

    public void Tick()
    {      

        PlayerMouseIdleCheck();
        LastCursorPosition = CursorPosition;
        LastDirectionVector = DirectionVector;

        MovementHistory();
    }
    private void MovementHistory()
    {
        _verticalHistory.Tick(Vertical, InputHelper.DeviceInputTool.IsUsingController());
        _horizontalHistory.Tick(Horizontal, InputHelper.DeviceInputTool.IsUsingController());
    }
    
    #endregion
    public void EnableMenuControls()
    {        
        
        PlayerInputActions.PlayerControls.Disable();
        PlayerInputActions.MenuControls.Enable();
        PlayerInputActions.LimitedMenuControls.Disable();
    }

    public void EnableGameplayControls()
    {

        PlayerInputActions.PlayerControls.Enable();
        PlayerInputActions.MenuControls.Disable();
        PlayerInputActions.LimitedMenuControls.Enable();
    }
    #region HandleEvents

    private void HandleStartScroll(InputAction.CallbackContext context)
    {
        var value = context.ReadValue<Vector2>();
        _scroll = value.y / 120; //Values from mouse output in 120 increments.
    }
    private void HandleEndScroll(InputAction.CallbackContext obj)
    {
        _scroll = 0f;
    }

    private void HandleJumpStart(InputAction.CallbackContext obj)
    {
        _jumpPressed = true;
    }

    private void HandleJumpEnd(InputAction.CallbackContext obj)
    {
        _jumpPressed = false;
    }
    
    public void ResetJump()
    {
        _jumpPressed=false;
    }

    

    private void HandleMouseAim(InputAction.CallbackContext context)
    {
        _mousePosition = context.ReadValue<Vector2>();
    }
    private void HandleMouseDeltaAim(InputAction.CallbackContext context)
    {
        _cursorDeltaPosition = context.ReadValue<Vector2>();
        _RawcursorDeltaPosition=_cursorDeltaPosition;
        if(_InputSettings==null)
            return;
        _cursorDeltaPosition.x*= _InputSettings.MouseXSensitivity;
        _cursorDeltaPosition.y*= _InputSettings.MouseYSensitivity;
    }

    private void HandleAnalogAim(InputAction.CallbackContext context)
    {
        
        //_analogAimPosition = context.ReadValue<Vector2>() * 15f;
        RawControllerAim= context.ReadValue<Vector2>();
        if(_InputSettings==null)
        {
            _analogAimPosition=RawControllerAim;
        }
        else
        {            
            _analogAimPosition = 
            new Vector2(_InputSettings.ControllerXAxisSensitivity*RawControllerAim.x,
                  _InputSettings.ControllerYAxisSensitivity*RawControllerAim.y);
        }

        
                    

    }
    private void HandleRunReleased(InputAction.CallbackContext obj)
    {
        _runPressed = false;
    }

    private void HandleRunPressed(InputAction.CallbackContext obj)
    {
        _runPressed = true;
    }


    #endregion

    #region Calculations
    public bool IsThereMovement()
    {
        float verticalAverage = _verticalHistory.Average();
        float horizontalAverage = _horizontalHistory.Average();

        bool isMovementhere = verticalAverage > 0.0025 || horizontalAverage > 0.0025;
        return isMovementhere;
    }

    private bool IsThereDifferenceInMovement()
    {
        return LastDirectionVector != DirectionVector;
    }
    private void PlayerMouseIdleCheck()
    {
        MouseIdleTimer.Tick();
        if (hasMouseMoved())
        {
            MouseIdleTimer.ResetTimer();
        }
    }

    private bool hasMouseMoved()
    {
        return LastCursorPosition != CursorPosition;
    }

    private void HotKeyCheck()
    {
        if (HotKeyPressed == null)
        {
            return;
        }

        for (int i = 0; i < 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                HotKeyPressed(i);
            }
        }
    }

    #endregion

    #region MiscMethods
    /*
    private static void DeviceChange(InputDevice device, InputDeviceChange change)
    {
        switch (change)
        {
            case InputDeviceChange.Added:
                // New Device.
                Debug.Log("Added: " + device.name);
                break;
            case InputDeviceChange.Disconnected:
                // Device got unplugged.
                Debug.Log("Disconnected: " + device.name);
                break;
            case InputDeviceChange.Reconnected:
                // Plugged back in.
                Debug.Log("Reconnected: " + device.name);
                break;
            case InputDeviceChange.Removed:
                // Remove from Input System entirely; by default, Devices stay in the system once discovered.
                Debug.Log("Removed: " + device.name);
                break;
            default:
                // See InputDeviceChange reference for other event types.
                break;
        }

    }*/

    private void UIImageSchemeInitialSet()
    {
        //Disables all devices currently read by InputSystem
        for (int rep = 0; rep < InputSystem.devices.Count - 1; rep++)
        {
            InputSystem.RemoveDevice(InputSystem.devices[rep]);
        }

        if (InputSystem.devices[0] == null) return;
        
        //Checks the first slot of the InputSystem devices list for controller type
        if (InputSystem.devices[0].description.manufacturer == "Sony Interactive Entertainment")
        {
            //Sets UI scheme to PS
            Debug.Log("Playstation Controller Detected");
            //currentImageScheme.SetImagesToPlaystation();
            CurrentUserControllerType = UserControllerType.PlayStation;
            //controllerTypeChange.Invoke();
        }
        else
        {
            //Sets UI scheme to XB
            Debug.Log("Xbox Controller Detected");
            //currentImageScheme.SetImagesToXbox();
            CurrentUserControllerType = UserControllerType.Xbox;
            //controllerTypeChange.Invoke();
        }
    }

    private void OnActionChange(object obj, InputActionChange change)
    {
        if (change == InputActionChange.ActionPerformed)
        {
            var inputAction = (InputAction)obj;
            var lastControl = inputAction.activeControl;
            var lastDevice = lastControl.device;

            string deviceName= lastDevice.name;

            //Assumes there's only one keyboard and Mouse
            if(lastDevice.name == KEYBOARDNAME || lastDevice.name== MOUSENAME)
            {
                deviceName=UserControllerType.PC.ToString();
            }


            if (_deviceUsing!=deviceName)
            {
                const string VECTOR2NAME = "Vector2";
                //Zeroed Vector. meaning not moving. So for gamepad it will activate this method even though its not moving
                if (inputAction.expectedControlType == VECTOR2NAME && inputAction.ReadValue<Vector2>() == Vector2.zero)
                {
                    return;
                }


                _deviceUsing = deviceName;
                if(deviceName==UserControllerType.PC.ToString())
                {
                    CurrentUserControllerType=UserControllerType.PC;
                }
                else
                {
                    const string SONYMANUFACTURENAME = "Sony Interactive Entertainment";
                    if(lastDevice.device.description.manufacturer== SONYMANUFACTURENAME)
                    {
                        CurrentUserControllerType=UserControllerType.PlayStation;
                    }
                    else //Default to XBOX
                    {                        
                        CurrentUserControllerType=UserControllerType.Xbox;
                    }
                }
                //Debug.Log($"OnActionChange device: {lastDevice.displayName}");
                //Debug.Log($"OnActionChange devicename: {lastDevice.device.description.manufacturer}");

            }


        }
    }


    #endregion

}
