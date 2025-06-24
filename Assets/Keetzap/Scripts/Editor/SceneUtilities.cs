using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Keetzap.EditorTools
{
    public class SceneUtilities : BaseEditorWindow
    {
        private bool _enableNegativeScales;
        private bool _includeYAxis;

        [MenuItem("Keetzap/Scene Utilities")]
        static void Init()
        {
            SceneUtilities window = (SceneUtilities)EditorWindow.GetWindow(typeof(SceneUtilities));
            
            SetMargings(10, 5, 5, 5);
            SetSize(window, 300, 400, 120, 120);
            window.titleContent.text = "Scene Utilities";
            window.Show();
        }

        protected sealed override void MainSection()
        {
            _enableNegativeScales = EditorGUILayout.Toggle(new GUIContent("Enable Negative Scales"), _enableNegativeScales);

            EditorGUILayout.Space(3);
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Add Random Transformations", GUILayout.Height(ButtonHeight)))
                {
                    ApplyTransformationsOnly();
                }
            }
            EditorGUILayout.EndHorizontal();

            Decorators.Separator();

            _includeYAxis = EditorGUILayout.Toggle(new GUIContent("Round also Y axis"), _includeYAxis);

            EditorGUILayout.Space(3);
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Round position", GUILayout.Height(ButtonHeight)))
                {
                    ApplyRoundPosition();
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void ApplyTransformationsOnly()
        {
            var selection = Selection.gameObjects;

            foreach (GameObject o in selection)
            {
                Vector3 randomRotation = new (0, 90 * Random.Range(0, 4), 0);
                o.transform.eulerAngles = randomRotation;

                if (_enableNegativeScales)
                {
                    Vector3 randomScale = new (2 * Random.Range(0, 2) - 1, 1, 2 * Random.Range(0, 2) - 1);
                    o.transform.localScale = randomScale;
                }
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        private void ApplyRoundPosition()
        {
            var selection = Selection.gameObjects;

            foreach (GameObject o in selection)
            {
                Vector3 currentPosition = o.transform.localPosition;
                currentPosition *= 10;
                currentPosition = new(Mathf.RoundToInt(currentPosition.x), _includeYAxis ? Mathf.RoundToInt(currentPosition.y) : currentPosition.y, Mathf.RoundToInt(currentPosition.z));
                currentPosition /= 10.0f;

                o.transform.localPosition = currentPosition;
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        [MenuItem("GameObject/Keetzap/Create Separator", false, 1)]
        private static void CreateSeparator()
        {
            _ = new GameObject("────────────────────────");
        }
    }
}
