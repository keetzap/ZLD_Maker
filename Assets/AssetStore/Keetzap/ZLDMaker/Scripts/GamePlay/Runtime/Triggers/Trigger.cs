using Keetzap.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    [RequireComponent(typeof(BoxCollider))]
    public abstract class Trigger : MonoBehaviour, IListener
    {
        public static class Fields
        {
            public static string Listeners => nameof(listeners);
            //public static string ActiveOnAwake => nameof(activeOnAwake);
            public static string ShowGizmos => nameof(showGizmos);
            public static string DebugColor => nameof(debugColor);
            public static string Opacity => nameof(opacity);
            public static string ShowDependencyLinew => nameof(showDependencyLine);
            public static string DependencyLineColor => nameof(dependencyLineColor);
        }

        public Action<object, Collider> OnTriggerEvent { get; set; }

        [SerializeField] protected List<Listeners> listeners = new();
        //[SerializeField] protected bool activeOnAwake;
        [SerializeField] protected bool showGizmos = true;
        [SerializeField] protected Color debugColor = Color.green;
        [SerializeField] protected float opacity = 0.5f;
        [SerializeField] protected bool showDependencyLine = true;
        [SerializeField] protected Color dependencyLineColor = Color.cyan;

        protected BoxCollider Collider => GetComponent<BoxCollider>();
        protected bool _hasBeenUsed;
        protected GameManager _gameManager;
        protected GameData _gameData;

        public virtual void Awake()
        {
            _gameManager = GameManager.Instance;
            _gameData = _gameManager.GameData;

            Collider.isTrigger = true;

            foreach (var listener in listeners)
            {
                listener.listener.AddListener(this);
            }
        }

        //private void Start()
        //{
        //    if (activeOnAwake)
        //        OnTrigger(Collider, null);
        //}

        public void OnTrigger(object sender, Collider other)
        {
            OnTriggerEvent?.Invoke(sender, other);
        }

        public virtual void OnDrawGizmos()
        {
            if (showGizmos)
            {
                Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
                _ = DrawCube(Collider, debugColor);
                Gizmos.matrix = Matrix4x4.identity;
            }

            if (showDependencyLine)
            {
                Gizmos.color = dependencyLineColor;
                foreach (var listener in listeners)
                {
                    CustomGizmos.DrawArrow(transform.position + Collider.center, listener.listener.transform.position, 0.3f, 30f, 1);
                }
            }
        }

        public Vector3 DrawCube(BoxCollider collider, Color debugColor)
        {
            Vector3 center = collider.center;
            Vector3 size = collider.size;

            Gizmos.color = new(debugColor.r, debugColor.g, debugColor.b, opacity);
            Gizmos.DrawSphere(center, 0.1f);
            Gizmos.DrawCube(center, size);

            Gizmos.color = debugColor;
            Gizmos.DrawWireCube(center, size);

            return center;
        }
    }
}