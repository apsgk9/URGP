using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interaction
{
    public abstract class AngledInteractor<T> : BaseInteractor<T> where T : IInteractable
    {

#if UNITY_EDITOR
        [SerializeField]
        private bool VisualizeAngles;
#endif

        [Header("Main")]
        [SerializeField]
        private LayerMask layerMask;
        [SerializeField]
        [Min(0.01f)]
        private float InitialCheckRadius = 1.5f;

        [SerializeField]
        [Min(0.01f)]
        private float RaycastedCheckRadius = 0.01f;
        private const float additionalCheckRadius = 0.001f;
        [SerializeField]
        private Vector3 Offset;

        [Header("Angle Limits")]
        [SerializeField]
        [Min(0)]
        [UnityEngine.Serialization.FormerlySerializedAs("HorizontalInteractAngleLimit")]
        public float HorizontalAngleLimit = 135f;
        [SerializeField]
        [Min(0)]
        [UnityEngine.Serialization.FormerlySerializedAs("VerticalUpInteractAngleLimit")]
        public float UpperAngleLimit = 90f;
        [SerializeField]
        [Min(0)]
        [UnityEngine.Serialization.FormerlySerializedAs("VerticalDownInteractAngleLimit")]
        public float LowerAngleLimit = 30f;
        [ReadOnly]
        public List<T> InteractablesOnFieldList;


        private bool _interactableFound 
        {
            get 
            {
                if(InteractablesOnFieldList==null)
                    return false;
                return InteractablesOnFieldList.Count>0;
            }                
        }

        private Vector3 _origin;
        //private Vector3 direction;

        //private List<Vector3> _directions;
        //private List<float> _hitDistances;
        private void Awake()
        {
            InteractablesOnFieldList=new List<T>();
        }

        virtual protected void OnValidate()
        {
            HorizontalAngleLimit = Mathf.Clamp(HorizontalAngleLimit, 0, 360);
            UpperAngleLimit = Mathf.Clamp(UpperAngleLimit, 0, 180);
            LowerAngleLimit = Mathf.Clamp(LowerAngleLimit, 0, 180);
        }
        // Update is called once per frame
        virtual protected void Update()
        {
            Setup();
            InteractablesOnFieldList= FindInteractables(_origin, InitialCheckRadius, layerMask, RaycastedCheckRadius, additionalCheckRadius);            
        }


        //Angle
        protected override bool isWithinSpecificBoundaries(Vector3 origin,Vector3 hitPosition,Vector3 normalizedDeltaToInteractable)
        {
            
            Vector3 horizontalflatDirection = Vector3.ProjectOnPlane(normalizedDeltaToInteractable, GetUpward()).normalized;
            Vector3 verticalflatDirection = Vector3.ProjectOnPlane(normalizedDeltaToInteractable, GetSide()).normalized;

            var HorizontalAngleDifference = Vector3.Angle(GetForward(), horizontalflatDirection);
            var VerticalAngleDifference = Vector3.SignedAngle(GetForward(), verticalflatDirection, GetSide());

            bool NotWithinHorizontalInteractAngleLimit = HorizontalAngleDifference > HorizontalAngleLimit / 2;
            bool NotWithinVerticalInteractAngleLimit = (VerticalAngleDifference >= 0) ? VerticalAngleDifference > (UpperAngleLimit) : -VerticalAngleDifference > (LowerAngleLimit);

            bool NotWithinSpecifiedAngles = NotWithinHorizontalInteractAngleLimit || NotWithinVerticalInteractAngleLimit;
            return !NotWithinSpecifiedAngles;
        }

        private void Setup()
        {
            HorizontalAngleLimit = Mathf.Clamp(HorizontalAngleLimit, 0, 360);
            UpperAngleLimit = Mathf.Clamp(UpperAngleLimit, 0, 180);
            LowerAngleLimit = Mathf.Clamp(LowerAngleLimit, 0, 180);
            _origin = transform.TransformPoint(Offset);
            //_interactableFound = false;
            InteractablesOnFieldList = new List<T>();
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (!VisualizeAngles)
                return;
            if (!Application.isPlaying)
            {
                Setup();
                FindInteractables(_origin, InitialCheckRadius, layerMask, RaycastedCheckRadius, additionalCheckRadius);
            }


            Gizmos.color = Color.yellow;
            HorizontalArc();
            VerticalArc();
            if (_interactableFound)
            {
                Gizmos.color = Color.red;
                for (int i = 0; i < InteractablesOnFieldList.Count; i++)
                {
                    UnityEditor.Handles.Label(InteractablesOnFieldList[i].gameObject.transform.position, InteractablesOnFieldList[i].gameObject.name);
                }

            }


        }

        private void HorizontalArc()
        {

            Color HandleColor = Color.blue;
            //HandleColor.a = 0.025f;
            UnityEditor.Handles.color = HandleColor;

            UnityEditor.Handles.DrawWireArc(_origin, GetUpward(), GetForward(), HorizontalAngleLimit / 2, InitialCheckRadius);
            UnityEditor.Handles.DrawWireArc(_origin, GetUpward(), GetForward(), -HorizontalAngleLimit / 2, InitialCheckRadius);

            Vector3 RightHorizontalDirection = Quaternion.AngleAxis(HorizontalAngleLimit / 2, GetUpward()) * GetForward();
            Vector3 LefttHorionzontalDirection = Quaternion.AngleAxis(-HorizontalAngleLimit / 2, GetUpward()) * GetForward();
            Debug.DrawLine(_origin, _origin + RightHorizontalDirection * InitialCheckRadius, HandleColor);
            Debug.DrawLine(_origin, _origin + LefttHorionzontalDirection * InitialCheckRadius, HandleColor);

        }

        private void VerticalArc()
        {
            Color HandleColor = Color.green;

            UnityEditor.Handles.color = HandleColor;
            UnityEditor.Handles.DrawWireArc(_origin, GetSide(), GetForward(), UpperAngleLimit, InitialCheckRadius);
            HandleColor = Color.red;
            UnityEditor.Handles.color = HandleColor;
            UnityEditor.Handles.DrawWireArc(_origin, GetSide(), GetForward(), -LowerAngleLimit, InitialCheckRadius);


            Vector3 VerticalUpDirection = Quaternion.AngleAxis(UpperAngleLimit, GetSide()) * GetForward();
            Vector3 VerticalDownDirection = Quaternion.AngleAxis(-LowerAngleLimit, GetSide()) * GetForward();


            Debug.DrawLine(_origin, _origin + VerticalUpDirection * InitialCheckRadius, Color.green);
            Debug.DrawLine(_origin, _origin + VerticalDownDirection * InitialCheckRadius, Color.red);
        }
#endif

        private Vector3 GetSide()
        {
            //negative in order to have positive values be up. Need transform.right
            return -transform.right;
        }

        private Vector3 GetForward()
        {
            return transform.forward;
        }
        private Vector3 GetUpward()
        {
            return transform.up;
        }
    }
}