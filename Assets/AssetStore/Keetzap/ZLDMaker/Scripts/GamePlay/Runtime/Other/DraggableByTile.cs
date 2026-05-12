using Keetzap.Feedback;
using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class DraggableByTile : MonoBehaviour, IDraggable
    {
        public static class Fields
        {
            public static string MoveOnSingleTile => nameof(moveOnSingleTile);
            public static string TypeOfDirection => nameof(typeOfDirection);
            public static string PushTimeThreshold => nameof(pushTimeThreshold);
            public static string TimeToReachNextTile => nameof(timeToReachNextTile);
            public static string AnimationCurve => nameof(animationCurve);
            public static string PushFeedback => nameof(pushFeedback);
            public static string StopFeedback => nameof(stopFeedback);
        }

        [System.Flags]
        public enum TypeOfDirection
        {
            None = 0,
            PositiveX = 1 << 1,
            NegativeX = 1 << 2,
            PositiveZ = 1 << 3,
            NegativeZ = 1 << 4
        }

        [SerializeField] private bool moveOnSingleTile = true;
        [SerializeField] private TypeOfDirection typeOfDirection = TypeOfDirection.None;
        [SerializeField] private float pushTimeThreshold = 0.3f;
        [SerializeField] private float timeToReachNextTile = 0.5f;
        [SerializeField] private AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private FeedbackSystem pushFeedback;
        [SerializeField] private FeedbackSystem stopFeedback;

        private PlayerController _playerController;
        private Vector3 _initPosition;
        private Vector3 _targetPosition;
        private float _curveStep;
        private float _pushTimeThreshold;
        private Vector3 _pushDirection;
        private TypeOfDirection _pushDirectionFlag;
        private float _thresholdTile = 0.01f;
        private bool _objectIsDragged;
        private bool _objectHasReachDestination;

        private void Awake()
        {
            InitializeTimeThreshold();
        }

        private void Update()
        {
            DragObjectOneSingleTile();
        }

        private void DragObjectOneSingleTile()
        {
            if (_objectIsDragged)
            {
                _curveStep += Time.deltaTime / timeToReachNextTile;
                float step = animationCurve.Evaluate(_curveStep);
                transform.position = Vector3.Lerp(_initPosition, _targetPosition, step);

                if (Mathf.Abs(Vector3.Distance(transform.position, _targetPosition)) <= _thresholdTile)
                {
                    transform.position = _targetPosition;
                    _objectIsDragged = false;
                    _curveStep = 0;

                    if (moveOnSingleTile)
                        _objectHasReachDestination = true;

                    if (stopFeedback != null)
                    {
                        stopFeedback.Play();
                    }
                }
            }
        }

        public void OnStartDragging(PlayerController playerController)
        {
            if (pushFeedback != null)
            {
                pushFeedback.Play();
            }

            _playerController = playerController;
            OnDraggingObjectOneSingleTile();
        }

        private void OnDraggingObjectOneSingleTile()
        {
            if (_objectIsDragged || _objectHasReachDestination) return;

            _pushTimeThreshold -= Time.fixedDeltaTime;

            if (_pushTimeThreshold <= 0 && IsValidPushDirectionVector())
            {
                _pushDirection = GetPushDirectionVector();
                _pushDirectionFlag = GetPushDirectionFlag(transform.position - _playerController.transform.position);

                if (typeOfDirection.HasFlag(_pushDirectionFlag))
                {
                    _initPosition = transform.position;
                    _targetPosition = transform.position + _pushDirection;
                    _objectIsDragged = true;
                }
            }
        }
        
        private bool IsValidPushDirectionVector() => Mathf.Abs((int)GetPushDirectionVector().x) + Mathf.Abs((int)GetPushDirectionVector().z) != 0;

        private Vector3 GetPushDirectionVector() => _playerController.transform.TransformDirection(Vector3.forward).normalized;

        TypeOfDirection GetPushDirectionFlag(Vector3 dir)
        {
            return Mathf.Abs(dir.x) > Mathf.Abs(dir.z) ? dir.x > 0 ? TypeOfDirection.PositiveX : TypeOfDirection.NegativeX : dir.z > 0 ? TypeOfDirection.PositiveZ : TypeOfDirection.NegativeZ;
        }

        public bool OnDragged() => _objectIsDragged;

        public bool OnPushed() => false;

        public void OnStopBehaviour()
        {
            InitializeTimeThreshold();
        }

        public void InitializeTimeThreshold()
        {
            _pushTimeThreshold = pushTimeThreshold;
        }
    }
}
