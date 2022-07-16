using System;
using Movement;
using UnityEngine;
using static LocomotionEnmus;

public interface ICharacter
{
    Vector3 DesiredCharacterVectorForward { get; }
    Vector3 MovementInput { get; }
    GameObject CharacterGameObject { get; }
    PlayerLocomotionBaseState CurrentLocomotionState { get; set; }
    IMover Mover { get; }
    ICeilingDetector CeilingDetector { get; }
    float AdditionalFallingSpeed { get; }
    float YGravity { get; }
    Vector3 Velocity { get; set; }
    LocomotionMode LocomotionMode { get; set; }
    AirborneMode AirborneMode { get; set; }
    Vector3 SavedMomentum { get; set; }
    float LocomotionSpeed { get; }
    float MaxVerticalSpeed { get; set; }
    bool IsMovementPressed { get; }
    bool IsJumpPressed { get; }
    float JumpDuration { get; }
    float JumpSpeed { get; }
    float WalkSpeed { get; }
    float RunSpeed { get; }
    float SprintSpeed { get; }
    float SlidingMaxVelocity { get; }
    float SlopeLimit { get; }
    float SlideGravity { get; }
    float AirFriction { get; }
    float GroundFriction { get; }
    Vector3 RootMotionDeltaVelocity { get; }

    event Action TriggerJump;

    void HaltCharacterController();
    void InvokeJump();
    bool IsGrounded();
    bool IsGroundTooSteep(Vector3 GroundNormal, GameObject characterGameObject, float slopeLimit);
    Vector3 Momentum(GameObject characterGameObject, Vector3 momentum, float gravity, float airFriction);
    void OnGroundContactLost();
    Vector3 SlidingMomentum(GameObject characterGameObject, Vector3 momentum, Vector3 GroundNormal, float slideGravity, float slidingMaxVelocity);
    void UpdateModifiers();
}
