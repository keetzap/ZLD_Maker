using System.Collections;
using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    public class CharacterDebug : MonoBehaviour
    {
        public bool showGizmos = true;
        public Color debugColor = new (0, 0.63f, 1);
        public float opacity = 0.5f;
        public float size = 0.1f;

        private void OnDrawGizmos()
        {
            if (!showGizmos) return;

            Vector3 center = transform.position;
            Gizmos.color = new(debugColor.r, debugColor.g, debugColor.b, opacity);
            Gizmos.DrawSphere(center, size);
            Gizmos.color = debugColor;
            Gizmos.DrawWireSphere(center, size);
        }
    }
}
