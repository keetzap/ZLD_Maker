using Keetzap.Utils;
using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class DraggableByPath : MonoBehaviour, IDraggable
    {
        public enum TargetPositionMode { Transform, LocalOffset }
        public static class Fields
        {
            public static string PushTimeThreshold => nameof(pushTimeThreshold);
            public static string TargetPositionMode => nameof(targetPositionMode);
            public static string TargetTransform => nameof(targetTransform);
            public static string TargetPositionVector => nameof(targetPositionVector);
            public static string LookAtTarget => nameof(lookAtTarget);
            public static string OffsetLookAtTarget => nameof(offsetLookAtTarget);
            public static string DraggableRenderer => nameof(draggableRenderer);
            public static string AllowBackwards => nameof(allowBackwards);
            public static string LockAtTheEnd => nameof(lockAtTheEnd);
            public static string UseSnap => nameof(useSnap);
            public static string SnapValue => nameof(snapValue);
        }

        [SerializeField] private float pushTimeThreshold = 0.3f;
        [SerializeField] private TargetPositionMode targetPositionMode = TargetPositionMode.Transform;
        [SerializeField] private Transform targetTransform;
        [SerializeField] private Vector3 targetPositionVector = new Vector3(0f, 0f, 1f);
        [SerializeField] private bool lookAtTarget;
        [SerializeField] private int offsetLookAtTarget;
        [SerializeField] private GameObject draggableRenderer;
        [SerializeField] private bool allowBackwards;
        [SerializeField] private bool lockAtTheEnd;
        [SerializeField] private bool useSnap = true;
        [SerializeField] private float snapValue = 0.5f;

        public TargetPositionMode TargetMode => targetPositionMode;
        public Vector3 TargetPositionOffset
        {
            get => targetPositionVector;
            set => targetPositionVector = value;
        }
        public bool UseSnap => useSnap;
        public float SnapValue => snapValue;
        
        private PlayerController _playerController;
        private BoxCollider _colliderTrigger;
        private Vector3 _initPosition;
        private Vector3 _targetPosition;
        private Vector3 _absoluteVector3Target;
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
            if (targetPositionMode == TargetPositionMode.LocalOffset)
            {
                _absoluteVector3Target = transform.TransformPoint(targetPositionVector);
            }
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

            if (lookAtTarget)
            {
                if (targetPositionMode == TargetPositionMode.Transform && targetTransform != null && targetTransform.GetComponentInParent<DraggableByTile>() == null)
                {
                    draggableRenderer.transform.LookAt(targetTransform, Vector3.up);
                    draggableRenderer.transform.eulerAngles += new Vector3(0, offsetLookAtTarget, 0);
                }
                else if (targetPositionMode == TargetPositionMode.LocalOffset)
                {
                    Vector3 worldTarget = transform.TransformPoint(targetPositionVector);
                    draggableRenderer.transform.LookAt(worldTarget, Vector3.up);
                    draggableRenderer.transform.eulerAngles += new Vector3(0, offsetLookAtTarget, 0);
                }
            }
        }

        public bool GetComponentInParent()
        {
            return targetTransform != null && targetTransform.GetComponentInParent<DraggableByTile>() == null;
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
                _targetPosition = (targetPositionMode == TargetPositionMode.Transform) ? targetTransform.position : (Application.isPlaying ? _absoluteVector3Target : transform.TransformPoint(targetPositionVector));

                _pushDirection = GetPushDirectionVector(_initPosition - _playerController.transform.position);
                _pathDirection = (targetPositionMode == TargetPositionMode.Transform ? targetTransform.position : (Application.isPlaying ? _absoluteVector3Target : transform.TransformPoint(targetPositionVector))) - _initPosition;
                _pushAngle = Functions.Remap(Vector3.Angle(_pushDirection, _pathDirection), 0, 90, 1, 0);
                
                if (targetTransform != null && targetTransform.parent != null)
                {
                    Transform currentParent = targetTransform.parent;
                    targetTransform.SetParent(null);
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
            if (targetTransform != null || targetPositionMode == TargetPositionMode.LocalOffset)
            {
                Gizmos.color = Color.cyan;
                
                if (targetPositionMode == TargetPositionMode.Transform && targetTransform != null)
                {
                    Gizmos.DrawLine(transform.position, targetTransform.position);
                    Gizmos.DrawSphere(targetTransform.position, 0.25f);
                }
                else
                {
                    Vector3 worldTarget = Application.isPlaying ? _absoluteVector3Target : transform.TransformPoint(targetPositionVector);
                    Gizmos.DrawLine(transform.position, worldTarget);
                    Gizmos.DrawSphere(worldTarget, 0.25f);
                }
                Gizmos.DrawSphere(Application.isPlaying ? _initPosition : transform.position, 0.25f);
            }
        }
    }
}
