using Keetzap.Feedback;
using System;
using System.Collections;
using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    public class Hitable : MonoBehaviour, IHitable
    {
        public static class Fields
        {
            public static string ConfigurationFile => nameof(configurationFile);
            public static string Model => nameof(model);
            public static string DamageFeedback => nameof(damageFeedback);
            public static string DeathFeedback => nameof(deathFeedback);
            public static string DestroyObject => nameof(destroyObject);
            public static string Destruction => nameof(typeOfDestruction);
            public static string Delay => nameof(delay);
        }

        [SerializeField] private GD_Hitable configurationFile;
        [SerializeField] private GameObject model;
        [SerializeField] private FeedbackSystem damageFeedback;
        [SerializeField] private FeedbackSystem deathFeedback;
        [SerializeField] private bool destroyObject;
        [SerializeField] private TypeOfDestruction typeOfDestruction = TypeOfDestruction.AfterFeedbackDuration;
        [SerializeField] private float delay;

        public Action<int, float> OnAttackedEvent { get; set; }
        public Action<int, float> OnDeadEvent { get; set; }
        public Action<Hitable> OnDestructionEvent { get; set; }

        private float _life;

        private void Awake()
        {
            _life = configurationFile.life;
        }

        public void OnAttacked(int damage, float power)
        {
            _life -= damage;

            if (_life > 0)
            {
                OnAttackedEvent?.Invoke(damage, power);
                OnAttack();
            }
            else
            {
                OnDeadEvent?.Invoke(damage, power);
                OnDestructionEvent?.Invoke(this);
                OnDead();
            }
        }

        public GD_Hitable GameDataAsset()
        {
            return configurationFile;
        }

        public void OnAttack()
        {
            if (damageFeedback != null)
            {
                damageFeedback.Play();
            }
        }

        public void OnDead()
        {
            if (deathFeedback != null)
            {
                deathFeedback.Play();
            }

            if (destroyObject)
            {
                StartCoroutine(DestroyHitable());
            }
        }
        
        public float GetDelayFromFeedback() => deathFeedback != null ? deathFeedback.totalDuration : 0;

        IEnumerator DestroyHitable()
        {
            Destroy(GetComponent<Collider>());

            float timer;

            if (typeOfDestruction == TypeOfDestruction.AfterDelay)
            {
                timer = delay;
            }
            else if (typeOfDestruction == TypeOfDestruction.Instantly)
            {
                timer = 0;
            }
            else
            {
                timer = GetDelayFromFeedback();
            }

            yield return new WaitForSeconds(timer + Time.deltaTime);

            model.SetActive(false);
            Destroy(gameObject, 1);
        }
    }

    [Serializable]
    public struct Destructible
    {
        public Hitable destructible;
    }
}