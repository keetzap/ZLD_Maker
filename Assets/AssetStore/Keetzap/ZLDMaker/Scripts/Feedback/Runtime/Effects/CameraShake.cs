/*
using Cinemachine;
using Keetzap.Utils;
using System.Collections;
using UnityEngine;

namespace Keetzap.Feedback
{
    [FeedbackEffect("Feedbacks/Camera Shake", 0.4f, 0.1f, 0.6f)]
    public class CameraShake : FeedbackEffect
    {
        new public static class Fields
        {
            public static string TypeOfAmplitudeValue => nameof(typeOfAmplitudeValue);
            public static string AmplitudeValue => nameof(amplitudeValue);
            public static string AmplitudeCurve => nameof(amplitudeCurve);
            public static string TypeOfFrequenceValue => nameof(typeOfFrequenceValue);
            public static string FrequencyValue => nameof(frequencyValue);
            public static string FrequencyCurve => nameof(frequencyCurve);
        }

        public CameraShake() : base("Camera Shake", false, false) { }

        [SerializeField] private TypeOfValue typeOfAmplitudeValue = TypeOfValue.AnimationCurve;
        [SerializeField] private float amplitudeValue = 3;
        [SerializeField] private AnimationCurve amplitudeCurve = new (new Keyframe(0, 0), new Keyframe(0.5f, 3), new Keyframe(1, 0));
        [SerializeField] private TypeOfValue typeOfFrequenceValue = TypeOfValue.ConstantValue;
        [SerializeField] private float frequencyValue = 20;
        [SerializeField] private AnimationCurve frequencyCurve = new (new Keyframe(0, 0), new Keyframe(0.5f, 5), new Keyframe(1, 0));

        private bool _startShaking;
        private CinemachineVirtualCamera _currentVC;
        private CinemachineBasicMultiChannelPerlin _noise;
        private float _curveStep = 0;
        private float _currentAmplitude;
        private float _currentFrequence;

        protected override IEnumerator OnPlay(float delay)
        {
            yield return new WaitForSeconds(delay);

            _currentVC = CinemachineCore.Instance.GetActiveBrain(0).ActiveVirtualCamera as CinemachineVirtualCamera;
            _noise = _currentVC.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            _currentAmplitude = _noise.m_AmplitudeGain;
            _currentFrequence = _noise.m_FrequencyGain;

            _startShaking = true;
        }

        private void Update()
        {
            if (_startShaking)
            {
                ShakeCamera();
            }
        }

        private void ShakeCamera()
        {
            _curveStep += Time.deltaTime / _duration;
            _noise.m_AmplitudeGain = typeOfAmplitudeValue == TypeOfValue.ConstantValue ? amplitudeValue : amplitudeCurve.Evaluate(_curveStep);
            _noise.m_FrequencyGain = typeOfFrequenceValue == TypeOfValue.ConstantValue ? frequencyValue : frequencyCurve.Evaluate(_curveStep);

            if (_curveStep >= _duration)
            {
                _curveStep = 0;
                _noise.m_AmplitudeGain = _currentAmplitude;
                _noise.m_FrequencyGain = _currentFrequence;
                _startShaking = false;
            }
        }
    }
}*/