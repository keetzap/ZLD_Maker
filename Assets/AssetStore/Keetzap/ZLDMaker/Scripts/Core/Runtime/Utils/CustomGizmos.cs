using UnityEngine;

namespace Keetzap.Core
{
    public static class CustomGizmos
    {
        public static void DrawArrow(Vector3 from, Vector3 to, float length, float angle, float position)
        {
            Arrow(from, to, length, angle, position);
        }

        public static void DrawArrow(Vector3 from, Vector3 to, Color color, float length, float angle, float position)
        {
            Gizmos.color = color;
            Arrow(from, to, length, angle, position);
        }

        public static void DrawSingleArrow(Vector3 from, Vector3 to, float length, float angle, float position)
        {
            Arrow(from, to, length, angle, position, true);
        }

        public static void DrawSingleArrow(Vector3 from, Vector3 to, Color color, float length, float angle, float position)
        {
            Gizmos.color = color;
            Arrow(from, to, length, angle, position, true);
        }

        private static void Arrow(Vector3 from, Vector3 to, float length, float angle, float position, bool singleArrow = false)
        {
            Gizmos.DrawLine(from, to);
            Vector3 direction = to - from;
            float thresholdDirection = 0.01f;

            if (direction.magnitude > thresholdDirection)
            {
                Vector3 arrow = from + (direction * position);

                Vector3 up = (Quaternion.LookRotation(direction) * Quaternion.Euler(0, angle, 0) * Vector3.back) * length;
                Vector3 down = (Quaternion.LookRotation(direction) * Quaternion.Euler(0, -angle, 0) * Vector3.back) * length;
                Gizmos.DrawRay(arrow, up);
                Gizmos.DrawRay(arrow, down);

                if (!singleArrow)
                {
                    Vector3 right = (Quaternion.LookRotation(direction) * Quaternion.Euler(angle, 0, 0) * Vector3.back) * length;
                    Vector3 left = (Quaternion.LookRotation(direction) * Quaternion.Euler(-angle, 0, 0) * Vector3.back) * length;
                    Gizmos.DrawRay(arrow, right);
                    Gizmos.DrawRay(arrow, left);
                }
            }
        }
    }
}