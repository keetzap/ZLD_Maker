using System;
using System.Collections.Generic;
using UnityEngine;

namespace Keetzap.Feedback
{
    public class FeedbackSystem : MonoBehaviour
    {
        public static class Fields
        { 
            public static string UnparentFeedback => nameof(unparentFeedback);
            public static string DestroyFeedbackAfterPlaying => nameof(destroyFeedbackAfterPlaying);
            public static string FeedbackList => nameof(_feedbacks);
        }

        public bool unparentFeedback;
        public bool destroyFeedbackAfterPlaying;

        [SerializeField] private FeedbackSystemSettings _feedbackPlayerSettings;
        [SerializeReference] private List<FeedbackEffect> _feedbacks = new();

        public List<FeedbackEffect> FeedbackEffects => _feedbacks;
        public float totalDuration;

        private void Awake()
        {
            foreach (var feedbackEffect in _feedbacks)
            {
                feedbackEffect.Initialize();
            }
        }

        public void Play()
        {
            if (unparentFeedback)
            {
                transform.parent = null;
            }

            foreach (var feedbackEffect in _feedbacks)
            {
                feedbackEffect.Play();
            }

            if (destroyFeedbackAfterPlaying)
            {
                Destroy(gameObject, totalDuration);
            }
        }

        public void PlayWithoutDelays()
        {
            foreach (var feedbackEffect in _feedbacks)
            {
                feedbackEffect.PlayWithoutDelay();
            }
        }

        public void Stop()
        {
            foreach (var feedbackEffect in _feedbacks)
            {
                feedbackEffect.Stop();
            }
        }
    }

    [Serializable]
    public class FeedbackSystemSettings
    {
        public enum TimeScale
        {
            Unscaled = 0,
            Scaled = 1
        }

        [SerializeField] private TimeScale _timeScale;
        public TimeScale TimeScaling => _timeScale;
    }
}