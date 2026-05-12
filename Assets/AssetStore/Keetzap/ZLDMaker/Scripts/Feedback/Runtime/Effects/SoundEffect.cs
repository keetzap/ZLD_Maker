using Keetzap.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Keetzap.Feedback
{
	[FeedbackEffect("Feedbacks/Play Sound", 0.9f, 0.5f, 0.0f)]
	public class SoundEffect : FeedbackEffect
	{
        new public static class Fields
        {
            public static string Clip => nameof(clip);
            public static string PlayRandomSound => nameof(playRandomSound);
            public static string RandomClips => nameof(randomClips);
            public static string Loop => nameof(loop);
            public static string MinVolume => nameof(minVolume);
            public static string MaxVolume => nameof(maxVolume);
            public static string MinPitch => nameof(minPitch);
            public static string MaxPitch => nameof(maxPitch);
        }

        public SoundEffect() : base("Play Sound", true, true) { }

        [SerializeField] private AudioClip clip;
        [SerializeField] private bool playRandomSound;
        [SerializeField] private List<AudioClip> randomClips = new List<AudioClip>();
        [SerializeField] private bool loop;
		[SerializeField] private float minVolume = 1f;
		[SerializeField] private float maxVolume = 1f;
		[SerializeField] private float minPitch = 1f;
		[SerializeField] private float maxPitch = 1f;

        protected override IEnumerator OnPlay(float delay)
        {
            yield return new WaitForSeconds(delay);

            float volume = Random.Range(minVolume, maxVolume);
            float pitch = Random.Range(minPitch, maxPitch);
            AudioClip _audioClip = (playRandomSound && randomClips.Count > 0) ? randomClips[Random.Range(0, randomClips.Count)] : clip;

			if (_audioClip != null)
            {
                if (loop)
                {
                    AudioManager.Instance.PlayMusic(_audioClip, Channel.Effects, volume, pitch, loop);
                }
                else
                {
                    AudioManager.Instance.PlayEffect(_audioClip, Channel.Effects, volume, pitch);
                }
            }
        }
    }
}