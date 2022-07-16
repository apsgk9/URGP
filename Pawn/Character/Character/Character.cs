using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controller;
using Movement;
using UnityEngine;
using static LocomotionEnmus;

public class Character : Pawn, ICharacter
{
    // gravity variables
    [Header("Character Options")]
    [SerializeField]
    [Min(0)]
    protected float _additionalFallingSpeed = 0;
    [SerializeField]
    [Min(0)]
    protected float _MaxVerticalSpeed = 10f;

    protected float _YGravity { get { return Physics.gravity.y; } }

    // state variables
    protected PlayerLocomotionBaseState _currentLocomotionState;
    protected PlayerLocomotionStateFactory _Locomotionstates;
    protected GameObject _characterGameObject;


    //Rotation
    public AnimationCurve _RotationBlend;
    public float RotationSpeed = 750f;
    protected IMover _Mover;
    public ICeilingDetector _ceilingDetector;
    protected List<CharacterModifier> _SortedModifiers;

    private IJumpUser[] JumpUsers { get; set; }

    //Friction

    public float _groundFriction = 100f;
    public float _airFriction = 0.5f;

    //sliding 

    protected float _slideGravity = 0.1f;
    [SerializeField]
    protected float _slidingMaxVelocity = 5f;

    protected float _slopeLimit = 45f;

    //Locomotion-
    protected bool _allowMovement = true;
    protected bool _forceMovementPressed = false;
    protected bool _IsMovementPressed { get { return GetIsThereMovement(); } }

    protected bool GetIsThereMovement()
    {
        if (_forceMovementPressed)
        {
            return true;
        }
        return _allowMovement && IsThereMovement;
    }

    public bool IsThereMovement;
    public bool AttemptToJump;

    protected bool _allowJumping = true;
    protected bool _isJumpPressed { get { return GetIsThereJump(); } }

    protected bool GetIsThereJump()
    {
        return _allowJumping && AttemptToJump;
    }

    protected Vector3 _savedMomentum;
    protected Vector3 _velocity;
    protected LocomotionMode _locomotionMode;
    protected AirborneMode _airborneMode;

    [SerializeField]
    protected float _WalkSpeed = 1.5f;


    [SerializeField]
    protected float _RunSpeed = 3f;

    [SerializeField]
    protected float _SprintSpeed = 6f;

    protected float _locomotionSpeed;
    protected Vector3 _movementInput;

    public Vector3 DesiredCharacterVectorForward { get; protected set; }
    protected float _angleDifference;


    //Jump
    [SerializeField]
    protected float _jumpSpeed = 3f;
    [SerializeField]
    [Min(0)]
    [Range(0, 1)]
    protected float _jumpDuration = 0.15f;
    [HideInInspector]
    public float CurrentJumpStartTime = 0f;
    [HideInInspector]
    public bool IsJumping;
    [HideInInspector]
    public bool JumpLock;
    public event Action TriggerJump;

    // getters and setters
    public float MovementVertical;
    public float MovementHorizontal;
    public Vector3 MovementInput { get { return _movementInput; } }
    public GameObject CharacterGameObject { get { return _characterGameObject; } }
    public PlayerLocomotionBaseState CurrentLocomotionState { get { return _currentLocomotionState; } set { _currentLocomotionState = value; } }
    public IMover Mover { get { return _Mover; } }
    public ICeilingDetector CeilingDetector { get { return _ceilingDetector; } }
    public float AdditionalFallingSpeed { get { return _additionalFallingSpeed; } }
    public float YGravity { get { return _YGravity; } }
    public Vector3 Velocity { get { return _velocity; } set { _velocity = value; } }
    public LocomotionMode LocomotionMode { get { return _locomotionMode; } set { _locomotionMode = value; } }
    public AirborneMode AirborneMode { get { return _airborneMode; } set { _airborneMode = value; } }
    public Vector3 SavedMomentum { get { return _savedMomentum; } set { _savedMomentum = value; } }
    public float LocomotionSpeed { get { return _locomotionSpeed; } }
    public float MaxVerticalSpeed { get { return _MaxVerticalSpeed; } set { _MaxVerticalSpeed = value; } }
    public Transform ViewTransform;
    //--Pressed--
    public bool IsMovementPressed { get { return _IsMovementPressed; } }
    public bool IsRunPressed;
    public bool IsJumpPressed { get { return _isJumpPressed; } }
    public float JumpDuration { get { return _jumpDuration; } }
    public float JumpSpeed { get { return _jumpSpeed; } }
    public float WalkSpeed { get { return _WalkSpeed; } }
    public float RunSpeed { get { return _RunSpeed; } }
    public float SprintSpeed { get { return _SprintSpeed; } }
    public float SlidingMaxVelocity { get { return _slidingMaxVelocity; } }
    public float SlopeLimit { get { return _slopeLimit; } }
    public float SlideGravity { get { return _slideGravity; } }
    public float AirFriction { get { return _airFriction; } }
    public float GroundFriction { get { return _groundFriction; } }
    public Vector3 RootMotionDeltaVelocity { get; protected set; }
    public bool ForceRootMotion=false;
    public bool ShouldUseRootMotion()
    {
        return RootMotionDeltaVelocity!=Vector3.zero || ForceRootMotion ;
    }

    [HideInInspector]
    public bool _LocomotionStateMachineCanTransition = true;

    public bool CanRotate = true;


    // Awake is called earlier than Start in Unity's event life cycle
    virtual protected void Awake()
    {
        //Velocities
        RootMotionDeltaVelocity = Vector3.zero;
        SavedMomentum = Vector3.zero;
        _velocity = Vector3.zero;

        _Mover = GetComponent<IMover>();
        _ceilingDetector = GetComponent<ICeilingDetector>();

        UpdateModifiers();
        JumpUsers = GetComponents<IJumpUser>();

        _characterGameObject = this.gameObject;

        //For facilitating rootmotion from an animator
        var _RootMotionDelta = _characterGameObject.GetComponentInChildren<RootMotionDeltaFixedUpdate>();
        if (_RootMotionDelta != null)
        {
            _RootMotionDelta.OnRootMotionUpdate += HandleRootMotion;
        }


        // Setup Locomotion HierarichalStateMachine
        _Locomotionstates = new PlayerLocomotionStateFactory(this);
        _locomotionMode = LocomotionMode.Idle;
        _airborneMode = AirborneMode.Grounded;
    }



    virtual protected void Start()
    {
        //Mover is not yet initialized in awake, this must be called in start
        _currentLocomotionState = _Locomotionstates.Grounded();
        _currentLocomotionState.Enter();
    }

    virtual protected void HandleRootMotion(Vector3 deltaPosition, float deltaTime, bool forceRootMotion)
    {
        //Override
        if (deltaTime == 0)
            return;
        ForceRootMotion=forceRootMotion;
        RootMotionDeltaVelocity = (deltaPosition / deltaTime);
    }

    public void ApplyRootMotion()
    {
        Velocity=RootMotionDeltaVelocity;
        RootMotionDeltaVelocity=Vector3.zero;
    }

    public void InvokeJump()
    {
        TriggerJump?.Invoke();
        if (JumpUsers == null)
            return;
        foreach (var jumpUser in JumpUsers)
        {
            jumpUser.HandleJump(this);
        }
    }



    // Update is called once per frame
    virtual protected void FixedUpdate()
    {
        HandleCharacterLocomotionMotion(_currentLocomotionState);
    }

    virtual protected void Update() 
    {

    }
    private void SetCharacterMotion()
    {
        //Rotation
        if (_movementInput != Vector3.zero && Mover.IsGrounded() && !IsJumping)
        {
            HandleRotation(DesiredCharacterVectorForward, CharacterGameObject.transform);
        }
    }

    private void GetUserInputForCharacter()
    {
        _movementInput = new Vector3(MovementHorizontal, 0, MovementVertical);
        DesiredCharacterVectorForward = CalculateCharacterDesiredVector(GetViewTransform(), _movementInput);
    }

    //TODO MAKE ROTATION SMOOTH.
    virtual protected void HandleCharacterLocomotionMotion(IBaseState baseStatetoUpdate)
    {
        ////Get Inputs
        GetUserInputForCharacter();
        SetCharacterMotion();

        //CheckGround
        Mover.CheckForGround();
        bool _isGrounded = Mover.IsGrounded();

        //Locomotion
        //Used to determine what locomotion mode the Player currently should be (Idle,Walk,Run,Sprint)
        RunLocomotion(baseStatetoUpdate);


        //If the character is grounded, extend ground detection sensor range;
        Mover.SetExtendSensorRange(_isGrounded);

        if (!IsJumping)
        {
            //Debug.Log("SM: "+SavedMomentum + " V: "+Velocity);
            //Debug.Log("SM.m: "+SavedMomentum.magnitude + " V.m: "+Velocity.magnitude);
            _Mover.SetVelocity(SavedMomentum + Velocity);
        }
        else
        {
            //Remove Ground Adjustment Velocity that interferes with jumping since it pushes it down
            _Mover.SetVelocity(SavedMomentum + Velocity - _Mover.GetCurrentGroundAdjustmentVelocity());
        }
    }

    


    private void RunLocomotion(IBaseState baseStatetoUpdate)
    {
        _locomotionMode = CalculateLocomotionMode(_locomotionMode, IsRunPressed, DesiredCharacterVectorForward, _movementInput);
        HandleModifiers();
        baseStatetoUpdate.UpdateStates();
    }

    //Get Transform that the character is going to move in relation to
    virtual protected Transform GetViewTransform()
    {
        if (ViewTransform != null)
        {
            return ViewTransform;
        }
        //By default it uses CharacterGameObject transform

        if (CharacterGameObject == null)
        {
            Debug.LogError("No CharacterGameObject found for GetViewTransform.");
            return this.transform;
        }
        return CharacterGameObject.transform;
    }

    virtual protected void OnCollisionEnter(Collision collision)
    {
        Vector3 firstContactPoint = collision.GetContact(0).point;

        if (_airborneMode == AirborneMode.Rising || _airborneMode == AirborneMode.Jumping)
            return;

        bool isAStaticObject = true;

        //Check if the rigidbody exists or is kinematic
        var rb = collision.gameObject.GetComponent<Rigidbody>();
        bool hasRigidBody = rb != null;
        if (hasRigidBody)
        {
            //Prevents non kinematic rigidbodies from stopping the momentum of the character
            isAStaticObject = rb.isKinematic == true;
        }
        if (isAStaticObject && _currentLocomotionState.ToString() == _Locomotionstates.Airborne().ToString())
        {
            SavedMomentum = Vector3.zero;
            Velocity = Vector3.zero;
        }
    }

    virtual protected void HandleModifiers()
    {
        if (_SortedModifiers.Count > 0)
        {
            foreach (var Modifier in _SortedModifiers)
            {
                Modifier.Handle(this);
            }
        }
    }



    virtual protected LocomotionMode CalculateLocomotionMode(LocomotionMode CurrentLocomotionMode, bool isSprinting, Vector3 DesiredCharacterVectorForward, Vector3 _movementInput)
    {

        const float runThreshold = 0.5f;
        const float sprintThreshold = 2.01f;

        var movementMagnitude = Mathf.Clamp(_movementInput.magnitude, 0, 1);

        int sprintModifierAddition = isSprinting ? 2 : 0;

        Vector3 runcomposite = DesiredCharacterVectorForward * sprintModifierAddition;

        Vector3 baseMovementComposite = (DesiredCharacterVectorForward * movementMagnitude);
        Vector3 finalMovementComposite = runcomposite + baseMovementComposite;
        var runGap = 0.00f;
        var walkGap = 0.00f;

        if (CurrentLocomotionMode == LocomotionMode.Run)
        {
            walkGap = -0.1f;
            runGap = -0.1f;
        }
        else if (CurrentLocomotionMode == LocomotionMode.Walk)
        {
            walkGap = 0.1f;
            runGap = -0.1f;
        }

        var finalMovementCompositeMagnitude = finalMovementComposite.magnitude;

        if (finalMovementCompositeMagnitude >= runThreshold + runGap && finalMovementCompositeMagnitude <= sprintThreshold) //run
        {
            CurrentLocomotionMode = LocomotionMode.Run;
            _locomotionSpeed = _RunSpeed;
        }
        else if (finalMovementCompositeMagnitude > sprintThreshold + Mathf.Epsilon) //sprint
        {
            CurrentLocomotionMode = LocomotionMode.Sprint;
            _locomotionSpeed = _SprintSpeed;
        }
        else if (finalMovementCompositeMagnitude < runThreshold + walkGap && finalMovementCompositeMagnitude > 0.01f) //walk
        {
            CurrentLocomotionMode = LocomotionMode.Walk;
            _locomotionSpeed = _WalkSpeed;
        }
        else
        {
            CurrentLocomotionMode = LocomotionMode.Idle;
            _locomotionSpeed = 0f;
        }
        return CurrentLocomotionMode;

    }

    //Rotation
    virtual protected void HandleRotation(Vector3 DesiredCharacterVectorForward, Transform characterTransform)
    {
        if (!CanRotate)
            return;
        
        Quaternion _finalRotation = RotateTransform(DesiredCharacterVectorForward, characterTransform, _RotationBlend, RotationSpeed);
        characterTransform.rotation = _finalRotation;

        //var deltaQuaternion = Quaternion.Inverse(characterTransform.rotation) * _finalRotation;
        //Mover.SetAngularVelocity(deltaQuaternion.eulerAngles);        

        
    }

    virtual protected Quaternion RotateTransform(Vector3 ForwardDirection, Transform TransformToRotate, AnimationCurve RotationBlend, float RotationSpeed)
    {
        var DesiredRotation = Quaternion.LookRotation(ForwardDirection);
        _angleDifference = Vector3.Angle(TransformToRotate.forward, ForwardDirection.normalized);

        var multiplier = 0f;
        multiplier = RotationBlend.Evaluate(_angleDifference / 180f);
       
        if (_angleDifference > 1)
        {
            return Quaternion.RotateTowards(TransformToRotate.rotation, DesiredRotation, GetDeltaTime() * RotationSpeed * multiplier);
        }
        return DesiredRotation;
    }
    //--------

    virtual protected Vector3 CalculateCharacterDesiredVector(Transform inputViewTransform, Vector3 movementInput)
    {
        return (Quaternion.Euler(0, inputViewTransform.eulerAngles.y, 0) * movementInput).normalized;
    }

    public bool IsGrounded()
    {
        return Mover.IsGrounded();
    }

    //Momentum

    public Vector3 SlidingMomentum(GameObject characterGameObject, Vector3 momentum, Vector3 GroundNormal, float slideGravity, float slidingMaxVelocity)
    {
        momentum = Vector3.ProjectOnPlane(momentum, GroundNormal);

        Vector3 _slideDirection = Vector3.ProjectOnPlane(-characterGameObject.transform.up, GroundNormal).normalized;
        momentum += _slideDirection * slideGravity * GetDeltaTime();
        momentum = Vector3.ClampMagnitude(momentum, slidingMaxVelocity);
        return momentum;
    }

    private static float GetDeltaTime()
    {
        if(Time.inFixedTimeStep)
            return Time.fixedDeltaTime;
        return Time.deltaTime;
    }

    public Vector3 Momentum(GameObject characterGameObject, Vector3 momentum, float gravity, float airFriction)
    {
        Vector3 _verticalMomentum = Vector3.zero;
        Vector3 _horizontalMomentum = Vector3.zero;

        //Split momentum into vertical and horizontal components;
        if (momentum != Vector3.zero)
        {
            _verticalMomentum = VectorMath.ExtractDotVector(momentum, characterGameObject.transform.up);
            _horizontalMomentum = momentum - _verticalMomentum;
        }
        if (_airborneMode != AirborneMode.Grounded)
        {
            //Add gravity to vertical momentum;
            _verticalMomentum += characterGameObject.transform.up * gravity * GetDeltaTime();
        }

        _horizontalMomentum = VectorMath.IncrementVectorTowardTargetVector(_horizontalMomentum, airFriction, GetDeltaTime(), Vector3.zero);


        //Add horizontal and vertical momentum back together;
        momentum = _horizontalMomentum + _verticalMomentum;

        return momentum;
    }

    public bool IsGroundTooSteep(Vector3 GroundNormal, GameObject characterGameObject, float slopeLimit)
    {
        return (Vector3.Angle(GroundNormal, characterGameObject.transform.up) > slopeLimit);
    }

    public void OnGroundContactLost()
    {
        SavedMomentum += Velocity;
        Velocity = Vector3.zero;
    }

    public void UpdateModifiers()
    {
        var unorderedLocomotionOverrides = GetComponents<CharacterModifier>();
        _SortedModifiers = unorderedLocomotionOverrides.OrderBy(o => o.priority).ToList();
    }

    virtual public void HaltCharacterController()
    {
        IsThereMovement = false;
        AttemptToJump = false;
        IsRunPressed = false;
        ViewTransform = null;
        MovementVertical = 0;
        MovementHorizontal = 0;
    }

}
