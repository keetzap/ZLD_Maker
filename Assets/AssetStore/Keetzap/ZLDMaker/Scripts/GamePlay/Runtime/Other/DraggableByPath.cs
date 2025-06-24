using Keetzap.Utils;
using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class DraggableByPath : MonoBehaviour, IDraggable
    {
        public static class Fields
        {
            public static string PushTimeThreshold => nameof(pushTimeThreshold);
            public static string TargetPosition => nameof(targetPosition);
            public static string LookAtTarget => nameof(lookAtTarget);
            public static string OffsetLookAtTarget => nameof(offsetLookAtTarget);
            public static string DraggableRenderer => nameof(draggableRenderer);
            public static string AllowBackwards => nameof(allowBackwards);
            public static string LockAtTheEnd => nameof(lockAtTheEnd);
        }

        [SerializeField] private float pushTimeThreshold = 0.3f;
        [SerializeField] private GameObject targetPosition;
        [SerializeField] private bool lookAtTarget;
        [SerializeField] private int offsetLookAtTarget;
        [SerializeField] private GameObject draggableRenderer;
        [SerializeField] private bool allowBackwards;
        [SerializeField] private bool lockAtTheEnd;
        
        private PlayerController _playerController;
        private BoxCollider _colliderTrigger;
        private Vector3 _initPosition;
        private Vector3 _targetPosition;
        private float _pushTimeThreshold;
        private bool _objectIsDragged;
        private bool _objectIsPushed;
        private bool _objectHasReachDestination;
        private Vector3 _pathDirection;
        private Vector3 _pushDirection;
        private float thresholdTile = 0.05f;
        private Vector3 _colliderCenter;
        private float _pushAngle;

        private void Awake()
        {
            InitializeTimeThreshold();
            SetTriggerCollider();

            _initPosition = transform.position;
        }

        private void SetTriggerCollider()
        {
            Component[] components = gameObject.GetComponents(typeof(BoxCollider));
            foreach (Component component in components)
            {
                if ((component as BoxCollider).isTrigger)
                {
                    _colliderTrigger = (BoxCollider)component;
                    _colliderCenter = _colliderTrigger.center;
                    return;
                }
            }
        }

        private void Update()
        {
            LookAtTargetPosition();
            DragObjectAlongPath();
        }

        private void DragObjectAlongPath()
        {
            if (_pushAngle < 0 && !allowBackwards) return;

            if (_objectIsPushed)
            {
                transform.position += _pathDirection.normalized * _playerController.PushSpeed * _pushAngle * Time.deltaTime;

                if (lockAtTheEnd && Vector3.Distance(transform.position, _targetPosition) < thresholdTile)
                {
                    transform.position = _targetPosition;
                    _objectIsPushed = false;
                    _objectHasReachDestination = true;
                    _colliderTrigger.size = Vector3.one;
                }
            }
        }

        public void LookAtTargetPosition()
        {
            if (Application.isPlaying || draggableRenderer == null) return;

            if (lookAtTarget && targetPosition != null && targetPosition.GetComponentInParent<DraggableByTile>() == null)
            {
                draggableRenderer.transform.LookAt(targetPosition.transform, Vector3.up);
                draggableRenderer.transform.eulerAngles += new Vector3(0, offsetLookAtTarget, 0);
            }
        }

        public bool GetComponentInParent()
        {
            return targetPosition != null && targetPosition.GetComponentInParent<DraggableByTile>() == null;
        }

        public void OnStartDragging(PlayerController playerController)
        {
            _playerController = playerController;
            OnDraggingObjectAlongPath();
        }

        private void OnDraggingObjectAlongPath()
        {
            if (_objectIsPushed) return;

            if (_objectHasReachDestination && lockAtTheEnd) return;

            _pushTimeThreshold -= Time.fixedDeltaTime;

            if (_pushTimeThreshold <= 0)
            {
                _targetPosition = targetPosition.transform.position;

                _pushDirection = GetPushDirectionVector(_initPosition - _playerController.transform.position);
                _pathDirection = targetPosition.transform.position - _initPosition;
                _pushAngle = Functions.Remap(Vector3.Angle(_pushDirection, _pathDirection), 0, 90, 1, 0);
                
                if (targetPosition.transform.parent != null)
                {
                    Transform currentParent = targetPosition.transform.parent;
                    targetPosition.transform.SetParent(null);
                }

                _colliderTrigger.center -= _pushDirection;
                _objectIsPushed = true;
            }
        }

        Vector3 GetPushDirectionVector(Vector3 dir)
        {
            return _playerController.transform.TransformDirection(Vector3.forward).normalized;
        }

        public bool OnDragged() => _objectIsDragged;

        public bool OnPushed() => _objectIsPushed;

        public void OnStopBehaviour()
        {
            InitializeTimeThreshold();
            _colliderTrigger.center = _colliderCenter;
            _objectIsPushed = false;
        }

        public void InitializeTimeThreshold()
        {
            _pushTimeThreshold = pushTimeThreshold;
        }

        void OnDrawGizmos()
        {
            if (targetPosition != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(transform.position, targetPosition.transform.position);
                Gizmos.DrawSphere(targetPosition.transform.position, 0.25f);
                Gizmos.DrawSphere(Application.isPlaying ? _initPosition : transform.position, 0.25f);
            }
        }
    }
}
