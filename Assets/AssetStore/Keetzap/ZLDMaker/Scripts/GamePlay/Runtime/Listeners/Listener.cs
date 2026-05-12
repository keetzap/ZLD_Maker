using Keetzap.Core;
using Keetzap.Feedback;
using System.Collections;
using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    [RequireComponent(typeof(Animator))]
    public class Listener : MonoBehaviour, IListener
    {
        public static class Fields
        {
            public static string InitialState => nameof(initialState);
            public static string HasCooldown => nameof(hasCooldown);
            public static string Cooldown => nameof(cooldown);
            public static string LoopBehavior => nameof(loopBehavior);
            public static string TypeOfEnd => nameof(typeOfEnd);
            public static string NumberOfOccurrences => nameof(numberOfOccurrences);
            public static string TimeOn => nameof(timeOn);
            public static string Interval => nameof(interval);
            public static string DelayOnTrigger => nameof(delayOnTrigger);
            public static string DelayOnRelease => nameof(delayOnRelease);
            public static string TriggerFeedback => nameof(triggerFeedback);
            public static string CooldownFeedback => nameof(cooldownFeedback);
            public static string ReleaseFeedback => nameof(releaseFeedback);
        }

        public readonly string ON = "On";
        public readonly string OFF = "Off";
        public readonly string INITSTATE = "InitialState";

        public enum TypeOfState
        {
            Disabled,
            Enabled
        }

        public enum TypeOfEnd
        {
            ByTriggerOrInteraction,
            AfterNumberOfOccurrences
        }

        public TypeOfState initialState = TypeOfState.Disabled;
        [SerializeField] private bool hasCooldown;
        [SerializeField] private float cooldown = 3;
        [SerializeField] private bool loopBehavior;
        [SerializeField] private TypeOfEnd typeOfEnd = TypeOfEnd.ByTriggerOrInteraction;
        [SerializeField] private int numberOfOccurrences = 1;
        [SerializeField] private float timeOn = 1;
        [SerializeField] private float interval = 1;
        [SerializeField] private float delayOnTrigger;
        [SerializeField] private float delayOnRelease;

        [SerializeField] private FeedbackSystem triggerFeedback;
        [SerializeField] private FeedbackSystem cooldownFeedback;
        [SerializeField] private FeedbackSystem releaseFeedback;

        private Trigger _trigger;
        protected TypeOfState _state;
        protected Animator _animator => GetComponent<Animator>();
        private Coroutine _startDelayOnTrigger;
        private Coroutine _startDelayOnRelease;
        private Coroutine _startCountdown;
        private Coroutine _triggerLoopBehaviour;
        private Coroutine _releaseLoopBehaviour;
        private WaitForSeconds _timeOn;
        private WaitForSeconds _interval;
        private int _numOccurences;

        private void Awake()
        {
            _timeOn = new WaitForSeconds(timeOn);
            _interval = new WaitForSeconds(interval);

            InitState();
        }

        private void InitState()
        {
            _state = initialState;
            _animator.SetFloat(INITSTATE, (int)_state);

            if (_state == TypeOfState.Enabled)
            {
                StartCooldownAndLoopBehaviour();
            }
        }

        public string GetCurrentState() => _state.ToString();

        public void AddListener(Trigger trigger)
        {
            _trigger = trigger;
            _trigger.OnTriggerEvent += OnTrigger;
        }

        private void OnDestroy()
        {
            if (_trigger != null)
            {
                _trigger.OnTriggerEvent -= OnTrigger;
            }
        }

        public virtual void OnTrigger(object sender, Collider other)
        {
            if (_state == TypeOfState.Disabled)
            {
                if (_startDelayOnRelease != null)
                {
                    StopCoroutine(_startDelayOnRelease);
                }
                else
                {
                    _startDelayOnTrigger = StartCoroutine(StartTriggerDelay(delayOnTrigger));
                }
            }
            else
            {
                if (_startDelayOnTrigger != null)
                {
                    StopCoroutine(_startDelayOnTrigger);
                }
                else
                {
                    _startDelayOnRelease = StartCoroutine(StartReleaselDelay(delayOnRelease));
                }
            }
        }

        IEnumerator StartTriggerDelay(float seconds)
        {
            yield return new WaitForSeconds(seconds);

            if (triggerFeedback != null)
            {
                triggerFeedback.Play();
            }

            StopAllCoroutines();
            _startDelayOnTrigger = null;

            SetBehavior(TypeOfState.Enabled, ON);

            StartCooldownAndLoopBehaviour();
        }

        IEnumerator StartReleaselDelay(float seconds)
        {
            yield return new WaitForSeconds(seconds);

            if (releaseFeedback != null)
            {
                releaseFeedback.Play();
            }
            
            StopAllCoroutines();
            _startDelayOnRelease = null;

            SetBehavior(TypeOfState.Disabled, OFF);
        }

        private void StartCooldownAndLoopBehaviour()
        {
            _numOccurences = numberOfOccurrences;
            StartCooldown();

            if (loopBehavior)
            {
                _releaseLoopBehaviour = StartCoroutine(ReleaseLoopBehaviour());
            }
        }

        IEnumerator ReleaseLoopBehaviour()
        {
            yield return _timeOn;

            SetBehavior(TypeOfState.Enabled, OFF);
            _numOccurences--;

            if (typeOfEnd == TypeOfEnd.AfterNumberOfOccurrences && _numOccurences == 0)
                yield break;

            _triggerLoopBehaviour = StartCoroutine(TriggerLoopBehaviour());
        }

        IEnumerator TriggerLoopBehaviour()
        {
            yield return _interval;

            SetBehavior(TypeOfState.Enabled, ON);
            _releaseLoopBehaviour = StartCoroutine(ReleaseLoopBehaviour());
        }

        private void SetBehavior(TypeOfState typeOfState, string trigger)
        {
            _state = typeOfState;

            if (!_animator.GetCurrentAnimatorStateInfo(0).IsName(trigger))
            {
                _animator.SetTrigger(trigger);
            }
        }
        
        public void StartCooldown()
        {
            if (hasCooldown)
            {
                if (_startCountdown != null)
                {
                    StopCoroutine(_startCountdown);
                    StopCoroutine(_triggerLoopBehaviour);
                    StopCoroutine(_releaseLoopBehaviour);
                }

                _startCountdown = StartCoroutine(StartCooldown(cooldown));

                if (cooldownFeedback != null)
                {
                    cooldownFeedback.Play();
                }
            }
        }

        IEnumerator StartCooldown(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            OnTrigger(null, null);
            _startCountdown = null;
            _state = TypeOfState.Disabled;
        }
    }

    [System.Serializable]
    public struct Listeners
    {
        public Listener listener;
    }
}

           