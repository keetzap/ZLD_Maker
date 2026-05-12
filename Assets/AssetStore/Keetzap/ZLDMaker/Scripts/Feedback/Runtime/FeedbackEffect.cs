using System;
using System.Collections;
//using System.Threading.Tasks;
using UnityEngine;

namespace Keetzap.Feedback
{
    [RequireComponent(typeof(FeedbackSystem))] 
    public abstract class FeedbackEffect : MonoBehaviour
    {
        public static class Fields
        {
            public static string Enabled => "m_Enabled";
            public static string Expanded => nameof(_expanded);
            public static string Delay => nameof(_delay);
            public static string IgnoreStopCoroutine => nameof(_ignoreStopCoroutine);
            public static string Duration => nameof(_duration);
            public static string DisplayLabel => nameof(_displayLabel);
            public static string DisplayColor => nameof(_displayColor);
            public static string ChildControlsDuration => nameof(_childControlsDuration);
        }

        public FeedbackEffect(string label, bool ignoreStopCoroutine, bool childControlsDuration, string message = "")
        {
            _displayLabel = label;
            _ignoreStopCoroutine = ignoreStopCoroutine;
            _childControlsDuration.childControlsDuration = childControlsDuration;
            _childControlsDuration.message = message;
        }

        [Serializable]
        public struct Duration
        {
            public bool childControlsDuration;
            public string message;
        }

        [SerializeField] private string _displayLabel;
        [SerializeField, ColorUsage(false, false)] private Color _displayColor = Color.black;
        [SerializeField, HideInInspector] private bool _expanded;
        [SerializeField] private float _delay;
        [SerializeField] protected float _duration;
        [SerializeField] protected bool _ignoreStopCoroutine;
        [SerializeField] protected Duration _childControlsDuration;

        private Coroutine _feedbackEffectCoroutine = null;

        protected float GetCurrentTime() => Time.time;
        public float GetFeedbackEffectDuration() => _delay + _duration;

        public virtual void Initialize()
        {
        }
        
        public void PlayWithoutDelay()
        {
            _feedbackEffectCoroutine = StartCoroutine(OnPlay(0));
        }

        //public async void Play()
        //{
        //    await Task.Delay(TimeSpan.FromSeconds(_delay));
        //    OnPlay();
        //}

        public void Play()
        {
            StopFeedbackEffect();
            _feedbackEffectCoroutine = StartCoroutine(OnPlay(_delay));
        }

        public void Stop()
        {
            OnStop();
            StopFeedbackEffect();
        }

        private void StopFeedbackEffect()
        {
            if (_feedbackEffectCoroutine != null && !_ignoreStopCoroutine)
            {
                StopCoroutine(_feedbackEffectCoroutine);
            }
        }

        protected abstract IEnumerator OnPlay(float delay);
        protected virtual void OnStop() { }
    }
}
