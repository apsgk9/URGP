using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Follow
{
    public static class FollowUtil
    {
        public static FollowHandler AddFollowScript(Vector3 Offset,Transform Anchor, GameObject resultGO, bool isTimeScaleIndependent,
        FollowType Type, float Smoothspeed)
        {
            var FollowScript = resultGO.AddComponent<FollowHandler>();
            if (Type == FollowType.Fixed)
            {
                FollowScript.Type = FollowType.Fixed;
            }
            else if (Type == FollowType.Smooth)
            {
                FollowScript.Type = FollowType.Smooth;
                FollowScript.SmoothSpeed = Smoothspeed;
            }

            FollowScript.Offset=Offset;

            FollowScript.ToFollow = Anchor;
            FollowScript.IndependentToTimeScale=isTimeScaleIndependent;
            return FollowScript;
        }

        public static FollowHandler AddFollowScript(Vector3 Offset,Transform Anchor, GameObject resultGO, bool isTimeScaleIndependent)
        {
            var FollowScript = resultGO.AddComponent<FollowHandler>();            
            FollowScript.Type = FollowType.Fixed;
            FollowScript.Offset=Offset;

            FollowScript.ToFollow = Anchor;
            FollowScript.IndependentToTimeScale=isTimeScaleIndependent;
            return FollowScript;
        }
    }

}
