using Keetzap.Core;
using Keetzap.Utils;
using System.Collections;
using UnityEngine;

namespace Keetzap.Feedback
{
    [FeedbackEffect("Feedbacks/Object Visibility", 0.2f, 0.7f, 0.9f)]
    public class ObjectVisibility : FeedbackEffect
    {
        new public static class Fields
        {
            public static string ObjectToEnable => nameof(objectToEnable);
            public static string Visibility => nameof(visibility);
        }

        public ObjectVisibility() : base("Object Visibility", true, true) { }

        [SerializeField] private GameObject objectToEnable;
        [SerializeField] private bool visibility = true;

        protected override IEnumerator OnPlay(float delay)
        {
            yield return new WaitForSeconds(delay);

            objectToEnable.SetActive(visibility);
        }
    }
}