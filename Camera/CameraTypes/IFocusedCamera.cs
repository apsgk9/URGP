using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    These Cameras focus on a specific gameobject in the scene.
*/
public interface IFocusedCamera
{
    Transform MainTransformToFocus { get; set; }
    void SetMainFocusTo(Transform target);
}
