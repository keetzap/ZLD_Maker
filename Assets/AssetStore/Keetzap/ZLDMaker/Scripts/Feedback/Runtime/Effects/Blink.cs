using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Keetzap.Feedback
{
    [System.Serializable]
    public class BlinkSequence
    {
        public float blinkDuration;
        public float blinkFrequency;
    }

    [FeedbackEffect("Feedbacks/Blink", 0f, 0.5f, 0.7f)]
    public class Blink : FeedbackEffect
    {
        new public static class Fields
        {
            public static string UseThisObject => nameof(useThisObject);
            public static string RenderObject => nameof(renderObject);
            public static string Sequence => nameof(sequence);
        }

        public Blink() : base("Blink", false, true, "Feedback duration is driven by the blink sequence parameters.") { }

        [SerializeField] private bool useThisObject;
        [SerializeField] private GameObject renderObject;
        [SerializeField] private List<BlinkSequence> sequence = new();

        private List<Renderer> _renderers = new();
        private WaitForSeconds _blinkFrequency;
        private Coroutine _renderFlicker = null;

        void Awake()
        {
            if (useThisObject)
            {
                _renderers = GetComponentsInChildren<Renderer>().ToList();
            }
            else
            {
                if (renderObject == null) return;

                _renderers = renderObject.GetComponentsInChildren<Renderer>().ToList();
            }
        }

        protected override IEnumerator OnPlay(float delay)
        {
            yield return new WaitForSeconds(delay);

            if (_renderers == null) yield break;

            _renderFlicker = StartCoroutine(RenderFlicker());
        }

        protected override void OnStop()
        {
            if (_renderFlicker != null)
            {
                StopCoroutine(_renderFlicker);
            }
        }

        public IEnumerator RenderFlicker()
        {
            for (int s = 0; s < sequence.Count; s++)
            {
                _blinkFrequency = new WaitForSeconds(sequence[s].blinkFrequency);

                float flickerStop = GetCurrentTime() + sequence[s].blinkDuration;

                while (GetCurrentTime() < flickerStop)
                {
                    SetRenderer(false);
                    yield return _blinkFrequency;

                    SetRenderer(true);
                    yield return _blinkFrequency;
                }
            }
        }

        private void SetRenderer(bool status)
        {
            for (int r = 0; r < _renderers.Count; r++)
            {
                _renderers[r].enabled = status;
            }
        }

        public void SetBlinkSequenceDuration()
        {
            float totalDuration = 0;

            for (int d = 0; d < sequence.Count; d++)
            {
                totalDuration += sequence[d].blinkDuration;
            }

            base._duration = totalDuration;
        }
    }
}