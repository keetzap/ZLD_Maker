using Keetzap.Core;
using Keetzap.Utils;
using System.Collections;
using UnityEngine;

namespace Keetzap.Feedback
{
    [FeedbackEffect("Feedbacks/Spawn Object", 0.0f, 0.9f, 0.2f)]
    public class SpawnObject : FeedbackEffect
    {
        new public static class Fields
        {
            public static string ObjectToSpawn => nameof(objectToSpawn);
            public static string Effect => nameof(effect);
        }

        public SpawnObject() : base("Spawn Object", true, true) { }

        [SerializeField] private GameObject objectToSpawn;
        [SerializeField] private SpawnObjectPosition effect;

        protected override IEnumerator OnPlay(float delay)
        {
            yield return new WaitForSeconds(delay);

            Vector3 anchor = effect.anchorType == TypeOfAnchor.Transform ? effect.anchorTransform.position : transform.position + effect.anchorOffset;

            _ = (GameObject)Instantiate(objectToSpawn, anchor, Quaternion.identity);
        }
    }
}