using Keetzap.Utils;
using System.Collections;
using UnityEngine;

namespace Keetzap.Feedback
{
    [FeedbackEffect("Feedbacks/Spin Object", 0.9f, 0.0f, 0.8f)]
    public class SpinObject : FeedbackEffect
    {
        new public static class Fields
        {
            public static string Model => nameof(model);
            public static string SpinSpeed => nameof(spinSpeed);
            public static string SpinAxis => nameof(spinAxis);
            public static string ReturnInitRotation => nameof(returnInitRotation);
            public static string UseAccelDeceleration => nameof(useAccelDeceleration);
            public static string SpinAccelDeceleration => nameof(spinAccelDeceleration);
            public static string AccelDecelValue => nameof(accelDecelValue);
            public static string AccelDecelCurve => nameof(accelDecelCurve);
        }

        public SpinObject() : base("Spin Object", false, false) { }

        [SerializeField] private GameObject model;
        [SerializeField] private float spinSpeed = 10f;
        [SerializeField] private TypeOfPositiveAxis spinAxis = TypeOfPositiveAxis.Y;
        [SerializeField] private bool returnInitRotation;

        [SerializeField] private bool useAccelDeceleration;
        [SerializeField] private TypeOfValue spinAccelDeceleration;
        [SerializeField] private float accelDecelValue = 3;
        [SerializeField] private AnimationCurve accelDecelCurve = new(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));

        private bool _startSpinning;
        private float _accelDecel = 1;
        private float _curveStep;
        private float _currentSpinSpeed;
        private float _spinStop;
        private Quaternion _initRotation;

        protected override IEnumerator OnPlay(float delay)
        {
            yield return new WaitForSeconds(delay);

            if (model == null) yield break;

            _startSpinning = true;
            _currentSpinSpeed = spinSpeed;
            _spinStop = GetCurrentTime() + _duration;
            _initRotation = model.transform.rotation;
        }

        private void Update()
        {
            if (_startSpinning)
            {
                CheckAccelDecel();
                Spin();
            }
        }

        private void Spin()
        {
            if (GetCurrentTime() < _spinStop && _currentSpinSpeed >= 0)
            {
                model.transform.Rotate(_accelDecel * _currentSpinSpeed * GetVector(spinAxis));
            }
            else
            {
                _startSpinning = false;
                _currentSpinSpeed = spinSpeed;
                _curveStep = 0;
                _accelDecel = 1;

                if (returnInitRotation)
                {
                    model.transform.rotation = _initRotation;
                }
            }
        }

        private void CheckAccelDecel()
        {
            if (!useAccelDeceleration) return;

            _curveStep += Time.deltaTime / _duration;

            if (spinAccelDeceleration == TypeOfValue.ConstantValue)
            {
                _currentSpinSpeed += accelDecelValue;
            }
            else
            {
                _accelDecel = accelDecelCurve.Evaluate(_curveStep);
            }
        }

        public Vector3 GetVector(TypeOfPositiveAxis axis)
        {
            switch (axis)
            {
                case TypeOfPositiveAxis.X: return Vector3.right;
                case TypeOfPositiveAxis.Y: return Vector3.up;
                case TypeOfPositiveAxis.Z: return Vector3.forward;
            };

            return Vector3.zero;
        }
    }
}