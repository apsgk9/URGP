using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interaction
{
    /*
        Uses Left Handed Coordinate System. Positive X is to the right. Positive Y is up
    */
    public struct ScreenInteractable<T>
    {
        public T Interactable;
        public Vector2 ScreenPosition;
        public ScreenInteractable(T i, Vector2 screenPos)
        {
            Interactable=i;
            ScreenPosition=screenPos;
        }
    }

    public abstract class ScreenInteractor<T> : MonoBehaviour where T : IInteractable
    {

#if UNITY_EDITOR
        [SerializeField]
        private bool VisualizeConstraints;
#endif
        

        [Header("Main")]
        [SerializeField]
        protected Transform Origin;
        [SerializeField]
        private LayerMask layerMask;
        [SerializeField]
        [Min(0.01f)]
        private float InitialCheckRadiusFromOrigin = 10f;

        [SerializeField]
        [Min(0.01f)]
        private float RaycastedCheckRadius = 0.01f;
        private const float additionalCheckRadius = 0.001f;
        public List<ScreenInteractable<T>> ScreenInteractables;
        protected bool InteractablesFound
        {
            get
            {
                if (ScreenInteractables == null)
                    return false;
                return ScreenInteractables.Count > 0;
            }
        }
        [SerializeField]

        private Camera _camera;
        [SerializeField]
        private Vector2 _CameraPixelResolution;
        [SerializeField]
        protected Vector2 CameraPixelResolution {get{return _CameraPixelResolution;}}
        [Header("Limits (Offset From Center)")]

        [Range(0, 1)]
        public float TopLimit = 0.9f;
        [Range(0, 1)]
        public float BottomLimit = 0.9f;
        [Range(0, 1)]
        public float LeftLimit = 0.9f;
        [Range(0, 1)]
        public float RightLimit = 0.9f;

        virtual protected void Update()
        {
            UpdateInteractables();
        }

        virtual protected void UpdateInteractables()
        {
            if (_camera == null)
            {
                GetPlayerCamera();
            }

            if (Origin == null || _camera == null)
                return;
            FindInteractables(GetOrigin(), InitialCheckRadiusFromOrigin, layerMask, RaycastedCheckRadius, additionalCheckRadius);
        }

        protected void FindInteractables(Vector3 origin, float initCheckRadius, LayerMask inputLayerMask, float inputRaycastedCheckRadius, float inputAdditionalCheckRadius)
        {
            ScreenInteractables = new List<ScreenInteractable<T>>();

            /*
                Checklist
                1.) Primary Check if interactables are within a certain distance calculated using a sphere sphere
                2.) Check if interactables are isWithinSpecificBoundaries.
                3.) Check if interactables are within are not blocked by colliders. (So it doesn't go through walls)
                    - Check if interactable is within the sphere that is casted
            */

            //Primary Check if interactables are within the sphere
            if (Physics.CheckSphere(origin, initCheckRadius))
            {
                Collider[] hitColliders = Physics.OverlapSphere(origin, initCheckRadius);
                foreach (var hitCollider in hitColliders)
                {
                    var interactable = hitCollider.GetComponent<T>();
                    if (interactable != null)
                    {
                        var normalizedDeltaFromCameraToInteractable = (hitCollider.transform.position - _camera.transform.position).normalized;

                        //Check if in Front/Back of camera 
                        float CameraDotObjectDelta=Vector3.Dot(normalizedDeltaFromCameraToInteractable,_camera.transform.forward);
                        if(CameraDotObjectDelta<0) // Behind camera
                            continue;
                        

                        Vector2 NormalizedScreenPosOfTarget = GetNormalizedScreenPosOfTarget(_camera, hitCollider.transform.position);
                        bool WithinSpecifiedBoundaries = WithinScreenSpaceBoundary(NormalizedScreenPosOfTarget);

                        if (!WithinSpecifiedBoundaries)
                            continue;
                        var normalizedDeltaToInteractable = (hitCollider.transform.position - origin).normalized;

                        //Check if interactables are within are not blocked by colliders.
                        var newDistance = Vector3.Distance(origin, hitCollider.transform.position);
                        RaycastHit hit;
                        if (Physics.SphereCast(origin, inputRaycastedCheckRadius, normalizedDeltaToInteractable, out hit, newDistance, inputLayerMask, QueryTriggerInteraction.UseGlobal))
                        {
                            //Check if interactable is within the sphere that is casted.
                            Vector3 stoppedPosition = origin + normalizedDeltaToInteractable * hit.distance;
                            Collider[] limitedColliders = Physics.OverlapSphere(stoppedPosition, inputRaycastedCheckRadius + inputAdditionalCheckRadius);

                            foreach (var limCol in limitedColliders)
                            {
                                if (limCol == hitCollider)
                                {
                                    ScreenInteractables.Add(new ScreenInteractable<T>(interactable,NormalizedScreenPosOfTarget));

                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void GetPlayerCamera()
        {
            _camera = CameraManager.Instance.GetPlayerCamera();
        }

        protected bool isWithinSpecificBoundaries(Vector3 origin, Vector3 hitPosition, Vector3 normalizedDeltaToInteractable)
        {
            Vector2 NormalizedScreenPosOfTarget = GetNormalizedScreenPosOfTarget(_camera, hitPosition);
            bool result = WithinScreenSpaceBoundary(NormalizedScreenPosOfTarget);
            return result;
        }

        private bool WithinScreenSpaceBoundary(Vector2 ScreenSpacePosition)
        {
            if (ScreenSpacePosition.y > TopLimit)
                return false;
            if (ScreenSpacePosition.y < -BottomLimit)
                return false;
            if (ScreenSpacePosition.x < -LeftLimit)
                return false;
            if (ScreenSpacePosition.x > RightLimit)
                return false;
            return true;
        }

        protected Vector2 GetNormalizedScreenPosOfTargetFromPlayerCam(Vector3 TargetPos)
        {
            GetPlayerCamera();
            return GetNormalizedScreenPosOfTarget(_camera,TargetPos);
        }

        private Vector2 GetNormalizedScreenPosOfTarget(Camera cam, Vector3 TargetPos)
        {
            _CameraPixelResolution.x = cam.pixelWidth;
            _CameraPixelResolution.y = cam.pixelHeight;
            Vector3 ScreenPoint = cam.WorldToScreenPoint(TargetPos, Camera.MonoOrStereoscopicEye.Mono);
            float normalizedX = ((ScreenPoint.x / _CameraPixelResolution.x) * 2) - 1;
            float normalizedY = ((ScreenPoint.y / _CameraPixelResolution.y) * 2) - 1;
            Vector2 NormalizedScreenPos = new Vector2(normalizedX, normalizedY);
            return NormalizedScreenPos;
        }
        virtual protected Vector3 GetOrigin()
        {
            return Origin.position;
        }
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (!VisualizeConstraints)
                return;
            if (Origin == null || _camera == null)
                return;
            if (!Application.isPlaying)
            {
                FindInteractables(GetOrigin(), InitialCheckRadiusFromOrigin, layerMask, RaycastedCheckRadius, additionalCheckRadius);
            }
            Gizmos.DrawWireSphere(GetOrigin(), InitialCheckRadiusFromOrigin);
            Gizmos.DrawSphere(GetOrigin(), 0.1f);

            if (InteractablesFound)
            {
                for (int i = 0; i < ScreenInteractables.Count; i++)
                {
                    UnityEditor.Handles.Label(ScreenInteractables[i].Interactable.gameObject.transform.position, ScreenInteractables[i].Interactable.gameObject.name);
                }
            }
        }
#endif
    }
}