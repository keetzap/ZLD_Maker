using System.Collections;
using UnityEngine;

namespace Keetzap.Feedback
{
    [FeedbackEffect("Feedbacks/Impulse Object", 0.45f, 0.7f, 0.1f)]
    public class ImpulseObject : FeedbackEffect
    {
        new public static class Fields
        {
            public static string Model => nameof(model);
            public static string TypeOfImpulse => nameof(typeOfImpulse);
            public static string ImpulseLinearForce => nameof(impulseLinearForce);
            public static string NegativeForce => nameof(impulseNegativeForce);
            public static string ImpulseCurveForce => nameof(impulseCurveForce);
            public static string CurveForce => nameof(curveForce);
        }

        public enum TypeOfImpulse
        {
            Linear,
            Curve
        }

        public ImpulseObject() : base("Impulse Object", false, false) { }

        private const float tangent = 4.42f;

        [SerializeField] private GameObject model;
        [SerializeField] private TypeOfImpulse typeOfImpulse;
        [SerializeField] private float impulseLinearForce = 0.1f;
        [SerializeField] private float impulseNegativeForce = -0.01f;
        [SerializeField] private float impulseCurveForce = 1f;
        [SerializeField] private AnimationCurve curveForce = new(new Keyframe(0, 0, tangent, tangent), new Keyframe(0.5f, 1), new Keyframe(1, 0, -tangent, -tangent));

        private bool _startImpulse;
        private Vector3 _initialPosition;
        private float _yPos;
        private float _currentImpulse;
        private float _curveStep;
        private float _impulseStop;

        protected override IEnumerator OnPlay(float delay)
        {
            yield return new WaitForSeconds(delay);

            if (model == null) yield break;

            _startImpulse = true;
            _initialPosition = model.transform.position;
            _yPos = _initialPosition.y;
            _currentImpulse = impulseLinearForce;
            _impulseStop = GetCurrentTime() + _duration;
        }

        private void Update()
        {
            if (_startImpulse)
            {
                AddImpulse();
            }
        }

        void AddImpulse()
        {
            if (typeOfImpulse == TypeOfImpulse.Linear)
            {
                _initialPosition.y += _currentImpulse;
                _currentImpulse += impulseNegativeForce;

                if (_initialPosition.y <= _yPos)
                {
                    _startImpulse = false;
                    _initialPosition.y = _yPos;
                    _currentImpulse = impulseLinearForce;
                }

                model.transform.position = _initialPosition;
            }
            else
            {
                if (GetCurrentTime() < _impulseStop)
                {
                    _curveStep += Time.deltaTime / _duration;
                    _currentImpulse = curveForce.Evaluate(_curveStep) * impulseCurveForce;
                    _initialPosition.y += _currentImpulse;
                    model.transform.position = _initialPosition;
                    _initialPosition.y = _yPos;
                }
                else
                {
                    _startImpulse = false;
                    model.transform.position = _initialPosition;
                    _curveStep = 0;
                }
            }
        }
    }
}