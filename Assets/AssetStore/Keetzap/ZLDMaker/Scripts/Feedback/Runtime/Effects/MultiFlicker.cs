using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Keetzap.Feedback
{
    [FeedbackEffect("Feedbacks/Multi Flicker", 0.9f, 0.9f, 0.0f)]
    public class MultiFlicker : FeedbackEffect
    {
        new public static class Fields
        {
            public static string RenderObject => nameof(renderObject);
            public static string MaterialStepList => nameof(materialStepList);
            public static string TimeStepPerMaterial => nameof(timeStepPerMaterial);
        }

        public MultiFlicker() : base("Multi Flicker", false, false) { }

        [SerializeField] private GameObject renderObject;
        [SerializeField] private Material[] materialStepList;
        [SerializeField] private float[] timeStepPerMaterial;

        private List<Renderer> _renderers = new();
        private List<Material[]> _defaultMaterials = new();

        void Start()
        {
            Renderer[] renderers = renderObject.GetComponentsInChildren<Renderer>();

            for (int r = 0; r < renderers.Length; r++)
            {
                _renderers.Add(renderers[r]);

                _defaultMaterials.Add(_renderers[r].materials);
            }

            if (materialStepList.Length != timeStepPerMaterial.Length)
            {
                Debug.LogError("ERROR : Material and Time lists must have the same number of elements.");
            }
        }

        protected override IEnumerator OnPlay(float delay)
        {
            yield return new WaitForSeconds(delay);

            if (_renderers == null) yield break;

            StartCoroutine(MultiFlick(materialStepList, timeStepPerMaterial));
        }

        public IEnumerator MultiFlick(Material[] materialStepList, float[] timeStepPerMaterial)
        {
            //flick materials
            for (int m = 0; m < materialStepList.Length; m++)
            {
                for (int r = 0; r < _renderers.Count; r++)
                {
                    Material[] materials = _renderers[r].materials;
                    Material[] flickMaterials = new Material[materials.Length];
                    System.Array.Copy(materials, flickMaterials, flickMaterials.Length);

                    for (int f = 0; f < flickMaterials.Length; f++)
                    {
                        flickMaterials[f] = materialStepList[m];
                    }

                    _renderers[r].materials = flickMaterials;
                }

                yield return new WaitForSeconds(timeStepPerMaterial[m]);
            }

            //restore default Materials
            for (int r = 0; r < _renderers.Count; r++)
            {
                _renderers[r].materials = _defaultMaterials[r];
            }

        }
    }
}