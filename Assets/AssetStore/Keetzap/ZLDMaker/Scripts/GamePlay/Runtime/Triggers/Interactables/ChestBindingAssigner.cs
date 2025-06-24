using Cinemachine;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Keetzap.ZeldaMaker
{
    [RequireComponent(typeof(PlayableDirector))]
    public class ChestBindingAssigner : MonoBehaviour
    {
        private readonly string CHARACTER = "Character Track";
        private readonly string CINEMACHINE = "Cinemachine Track";

        void Start()
        {
            PlayableDirector director = GetComponent<PlayableDirector>();
            TimelineAsset timeline = (TimelineAsset)director.playableAsset;

            foreach (var track in timeline.GetOutputTracks())
            {
                if (track.name == CHARACTER)
                {
                    director.SetGenericBinding(track, PlayerController.Instance.MainCharacterAnimator);
                }
                else if (track.name == CINEMACHINE)
                {
                    director.SetGenericBinding(track, Camera.main.GetComponent<CinemachineBrain>());
                }
            }
        }
    }
}