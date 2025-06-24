using Keetzap.Utils;
using System;
using UnityEngine;

namespace Keetzap.Core
{
    [Serializable]
    public struct SpawnObjectPosition
    {
        public TypeOfAnchor anchorType;
        public Transform anchorTransform;
        public Vector3 anchorOffset;
    }
}