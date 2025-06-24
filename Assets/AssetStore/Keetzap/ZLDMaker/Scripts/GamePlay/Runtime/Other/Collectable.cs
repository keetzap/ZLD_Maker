using Keetzap.Feedback;
using System.Collections;
using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    public class Collectable : MonoBehaviour, ICollectable
    {
        public static class Fields
        {
            public static string ConfigurationFile => nameof(configurationFile);
            public static string AwakeFeedback => nameof(awakeFeedback);
            public static string CollectFeedback => nameof(collectFeedback);
            public static string AutodestroyObject => nameof(autodestroyObject);
            public static string Destruction => nameof(typeOfDestruction);
            public static string Delay => nameof(delay);
        }

        public GD_Collectable configurationFile;
        [SerializeField] private FeedbackSystem awakeFeedback;
        [SerializeField] private FeedbackSystem collectFeedback;
        public bool autodestroyObject;
        [SerializeField] private TypeOfDestruction typeOfDestruction = TypeOfDestruction.AfterFeedbackDuration;
        [SerializeField] private float delay;

        private Coroutine _autodestroyCollectable = null;

        public virtual void Awake()
        {
            if (awakeFeedback != null)
            {
                awakeFeedback.Play();
            }

            if (autodestroyObject)
            {
                _autodestroyCollectable = StartCoroutine(Destroy());
            }
        }

        public GD_Collectable GameDataAsset()
        {
            return configurationFile;
        }

        public float GetDelayFromFeedback() => awakeFeedback != null ? awakeFeedback.totalDuration : 0;

        IEnumerator Destroy()
        {
            float timer = typeOfDestruction == TypeOfDestruction.AfterDelay ? delay : typeOfDestruction == TypeOfDestruction.Instantly ? 0 : GetDelayFromFeedback();
            yield return new WaitForSeconds(timer + Time.deltaTime);

            DestroyCollectable();
        }

        public virtual void OnCollected()
        {
            if (collectFeedback != null)
            {
                collectFeedback.Play();
            }

            GameManager.Instance.AddCollectable(configurationFile);
            DestroyCollectable();
        }

        public void DestroyCollectable()
        {
            if (_autodestroyCollectable != null)
            {
                if (awakeFeedback != null)
                {
                    awakeFeedback.Stop();
                }

                if (collectFeedback != null)
                {
                    collectFeedback.Stop();
                }

                StopCoroutine(_autodestroyCollectable);
            }

            StartCoroutine(DestroyThis());
        }

        private IEnumerator DestroyThis()
        {
            yield return new WaitForSeconds(Time.deltaTime);
            Destroy(gameObject);
        }
    }
}
