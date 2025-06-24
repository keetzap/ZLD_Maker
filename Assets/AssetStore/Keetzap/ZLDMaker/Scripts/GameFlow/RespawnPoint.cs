using Keetzap.Core;
using System;
using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    [RequireComponent(typeof(BoxCollider))]
    public class RespawnPoint : MonoBehaviour
    {
        public Action<RespawnPoint> OnDropByEvent;

        public static class Fields
        {
            public static string ShowGizmos => nameof(showGizmos);
            public static string Opacity => nameof(opacity);
            public static string SpawnColor => nameof(spawnColor);
            public static string SpawnRadius => nameof(spawnRadius);
            public static string HeightBox => nameof(heightBox);
            public static string ArrowLength => nameof(arrowLength);
        }

        [SerializeField] private bool showGizmos = true;
        [SerializeField] private float opacity = 0.7f;
        [SerializeField] protected Color spawnColor = new Color(1, 0.5f, 0);
        [SerializeField] private float spawnRadius = 0.2f;
        [SerializeField] private float heightBox = 0.5f;
        [SerializeField] private float arrowLength = 0.5f;


        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(StringsData.PLAYER))
            {
                OnDropByEvent?.Invoke(this);
            }
        }
        
        private float _half = 0.5f;

        private void OnDrawGizmos()
        {
            if (!showGizmos) return;

            Gizmos.color = spawnColor;
            Gizmos.DrawWireSphere(transform.position, spawnRadius);

            Vector3 wireCubePosition = new Vector3(transform.position.x, transform.position.y + heightBox * _half, transform.position.z);
            Gizmos.DrawWireCube(wireCubePosition, new(1, heightBox, 1));

            Vector3 initArrowPos = new Vector3(transform.position.x, transform.position.y + heightBox * _half, transform.position.z);
            CustomGizmos.DrawSingleArrow(initArrowPos + transform.TransformDirection(Vector3.forward) * _half, initArrowPos + transform.TransformDirection(Vector3.forward) * (_half + arrowLength), .25f, 30, 1);

            Color color = spawnColor;
            color.a = opacity;
            Gizmos.color = color;
            Gizmos.DrawSphere(transform.position, spawnRadius);
        }
    }   
}
