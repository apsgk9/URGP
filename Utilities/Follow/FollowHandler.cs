using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Follow
{
    public class FollowHandler : MonoBehaviour
    {
        public static readonly string GameObjectParentName ="FollowHandlers";
        public static GameObject GameObjectParent;
        public Transform ToFollow;
        public FollowType Type;
        public float SmoothSpeed;
        public Vector3 Offset;

        public bool IndependentToTimeScale=false;

        private void Awake()
        {
            //ParentToContainer();
            UtilityFunctions.GameObjectUtil.ParentToContainer(ref GameObjectParent,GameObjectParentName,transform);
        }
        private void Start()
        {
            ResetFollow();
        }

        //Should be called whenever the player moves somewhere else
        public void ResetFollow()
        {
            if(ToFollow==null)
                return;
            transform.position = CalculateFixedFollow();
        }
        private void Update()
        {
            if(ToFollow==null)
                return;

            if(Type==FollowType.Fixed)
            {
                transform.position= CalculateFixedFollow();
            }
            else if(Type==FollowType.Smooth)
            {
                transform.position= CalculateSmoothFollow();
            }
            else
            {
                Debug.LogError("Follow Type cannot be found.");
            }
        }

        private Vector3 CalculateSmoothFollow()
        {
            return Vector3.Lerp(transform.position, ToFollow.position + Offset, SmoothSpeed * GetDeltaTime());
        }

        private float GetDeltaTime()
        {
            if(IndependentToTimeScale)
            {
                return Time.unscaledDeltaTime;
            }
            return Time.deltaTime;
        }

        private Vector3 CalculateFixedFollow()
        {
            return ToFollow.position+Offset;
        }
    }
}
