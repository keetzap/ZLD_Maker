using System.Collections;
using UnityEngine;

namespace Keetzap.Feedback
{
    [FeedbackEffect("Feedbacks/Play Animation", 0f, 0.25f, 0.95f)]
    public class PlayAnimation : FeedbackEffect
    {
        new public static class Fields
        {
            public static string Animator => nameof(animator);
            public static string StateName => nameof(stateName);
        }

        public PlayAnimation() : base("Play Animation", false, true, "Unfortunately, Unity only can provide the length (time) of the clip that is currenly playing.") { }

        [SerializeField] private Animator animator;
        [SerializeField] private string stateName;

        protected override IEnumerator OnPlay(float delay)
        {
            yield return new WaitForSeconds(delay);

            if (animator == null) yield break;

            animator.Play(Animator.StringToHash(stateName));
        }
    }
}