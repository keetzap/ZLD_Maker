using Keetzap.ZeldaMaker;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

namespace Keetzap.Feedback
{
    [FeedbackEffect("Feedbacks/Play Timeline", 1f, 0.2f, 0.1f)]
    public class PlayTimeline : FeedbackEffect
    {
        new public static class Fields
        {
            public static string PlayableDirector => nameof(playableDirector);
        }

        public PlayTimeline() : base("Play Timeline", true, true, "The duration of this effect is driven by the length of the timeline.") { }

        [SerializeField] private PlayableDirector playableDirector;

        protected override IEnumerator OnPlay(float delay)
        {
            yield return new WaitForSeconds(delay);

            if (playableDirector == null) yield break;

            playableDirector.Play();
        }

        public void SetTimelineDuration()
        {
            if (playableDirector == null) return;

            base._duration = (float)playableDirector.duration;
        }
    }
}