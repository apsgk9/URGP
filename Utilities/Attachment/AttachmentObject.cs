using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AddressableAssets;

namespace Attachment
{
    [Serializable]
    public class AttachmentObject
    {
        public AssetReferenceGameObject AssetRefObject;
        public Transform Anchor;
        public Vector3 Offset;
        public Follow.FollowType Type=Follow.FollowType.Fixed;
        [Min(0)]
        public float SmoothSpeed=1f;
    }
}

