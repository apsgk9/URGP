using System;
using UnityEngine;

public interface IMover
{
    void CheckForGround();
    Collider GetGroundCollider();
    Vector3 GetGroundNormal();
    Vector3 GetGroundPoint();
    bool IsGrounded();
    void RecalculateColliderDimensions();
    void SetColliderHeight(float _newColliderHeight);
    void SetExtendSensorRange(bool _isExtended);
    void SetStepHeightRatio(float _newStepHeightRatio);
    void SetVelocity(Vector3 _velocity);
    void SetAngularVelocity(Vector3 _angularVelocity);
    Vector3 GetCurrentGroundAdjustmentVelocity();
    Vector3 GetVelocity();
}
