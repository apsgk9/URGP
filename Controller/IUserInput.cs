using System;
using UnityEngine;

public interface IUserInput
{
    event Action<int> HotKeyPressed;
    float Vertical {get;}
    float Horizontal {get;}
    float Scroll {get;}
    void Tick();
    bool JumpPressed {get;} // Indicates if jump is pressed down, resets when jump is released
    void ResetJump(); //
    bool RunPressed {get;}
    Vector2 CursorPosition { get;}
    Vector2 ControllerCursorDeltaPosition { get;}
    bool isPlayerLookIdle { get; }
    bool IsThereMovement();
    string DeviceUsing{get;}
}