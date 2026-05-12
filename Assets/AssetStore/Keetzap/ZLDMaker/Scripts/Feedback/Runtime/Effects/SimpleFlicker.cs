using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Keetzap.Feedback
{
    [FeedbackEffect("Feedbacks/Simple Flicker", 0.2f, 0.9f, 0.75f)]
    public class SimpleFlicker : FeedbackEffect
    {
        new public static class Fields
        {
            public static string RenderObject => nameof(renderObject);
            public static string FlickerFrequence => nameof(flickerFrequence);
            public static string ColorReplacement => nameof(colorReplacement);
            public static string FlickerColor => nameof(flickerColor);
            public static string FlickerMaterial => nameof(flickerMaterial);
        }

        public SimpleFlicker() : base("Simple Flicker", false, false) { }

        public enum TypeOfRenderReplacement
        {
            ByColor,
            ByMaterial
        }

        [SerializeField] private GameObject renderObject;
        [SerializeField] private float flickerDuration = 1;
        [SerializeField] private float flickerFrequence = 0.1f;
        [SerializeField] private TypeOfRenderReplacement colorReplacement;
        [SerializeField] private Color flickerColor = Color.red;
        [SerializeField] private Material flickerMaterial;

        private List<GameObject> _renderers = new();
        private List<Material> _currentMaterials = new();
        private List<Color> _currentColors = new();

        private Coroutine[] _coroutines;
        private WaitForSeconds _waitForSeconds;

        void Start()
        {
            Renderer[] renderers = renderObject.GetComponentsInChildren<Renderer>();

            for (int r = 0; r < renderers.Length; r++)
            {
                _renderers.Add(renderers[r].gameObject);
            }

            foreach (var render in renderers)
            {
                for (int r = 0; r < render.materials.Length; r++)
                {
                    _currentMaterials.Add(render.materials[r]);
                    _currentColors.Add(render.materials[r].color);
                }
            }

            _coroutines = new Coroutine[_currentMaterials.Count];
            _waitForSeconds = new WaitForSeconds(flickerFrequence);
        }

        protected override IEnumerator OnPlay(float delay)
        {
            yield return new WaitForSeconds(delay);

            if (_renderers == null) yield break;

            if (colorReplacement == TypeOfRenderReplacement.ByColor)
            {
                for (int m = 0; m < _currentMaterials.Count; m++)
                {
                    _coroutines[m] = StartCoroutine(FlickerColor(m, _currentColors[m]));
                }
            }
            else
            {
                for (int r = 0; r < _renderers.Count; r++)
                {
                    Renderer renderer = _renderers[r].GetComponent<Renderer>();
                    Material[] materials = renderer.materials;
                    Material[] flickerMaterials = new Material[materials.Length];
                    System.Array.Copy(materials, flickerMaterials, flickerMaterials.Length);

                    for (int m = 0; m < materials.Length; m++)
                    {
                        flickerMaterials[m] = flickerMaterial;
                    }

                    _coroutines[r] = StartCoroutine(FlickerMaterial(renderer, materials, flickerMaterials));
                }
            }
        }

        public IEnumerator FlickerColor(int materialIndex, Color initialColor)
        {
            float flickerStop = GetCurrentTime() + _duration;

            while (GetCurrentTime() < flickerStop)
            {
                _currentMaterials[materialIndex].color = flickerColor;
                yield return _waitForSeconds;
                _currentMaterials[materialIndex].color = initialColor;
                yield return _waitForSeconds;
            }

            _currentMaterials[materialIndex].color = initialColor;
        }

        public IEnumerator FlickerMaterial(Renderer renderer, Material[] materials, Material[] flickerMaterials)
        {
            float flickerStop = GetCurrentTime() + _duration;

            while (GetCurrentTime() < flickerStop)
            {
                renderer.materials = flickerMaterials;
                yield return _waitForSeconds;
                renderer.materials = materials;
                yield return _waitForSeconds;
            }

            renderer.materials = materials;
        }

        private float GetCurrentTime() => Time.time;
    }
}