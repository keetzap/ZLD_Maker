using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Keetzap.EditorTools
{
    public class SceneUtilities : BaseEditorWindow
    {
        internal static bool EnableNegativeScales;
        internal static bool IncludeYAxis;
        internal static float SnapStep = 0.5f;

        [MenuItem("Keetzap/Scene Utilities")]
        static void Init()
        {
            SceneUtilities window = (SceneUtilities)EditorWindow.GetWindow(typeof(SceneUtilities));
            
            SetMargings(10, 5, 5, 5);
            SetSize(window, 300, 400, 150, 150);
            window.titleContent.text = "Scene Utilities";
            window.Show();
        }

        protected sealed override void MainSection()
        {
            EnableNegativeScales = EditorGUILayout.Toggle(new GUIContent("Enable Negative Scales"), EnableNegativeScales);

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

            IncludeYAxis = EditorGUILayout.Toggle(new GUIContent("Round also Y axis"), IncludeYAxis);
            SnapStep = EditorGUILayout.FloatField(new GUIContent("Snap Step"), SnapStep);

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

        internal static void ApplyTransformationsOnly()
        {
            var selection = Selection.gameObjects;

            foreach (GameObject o in selection)
            {
                Vector3 randomRotation = new (0, 90 * Random.Range(0, 4), 0);
                o.transform.eulerAngles = randomRotation;

                if (EnableNegativeScales)
                {
                    Vector3 randomScale = new (2 * Random.Range(0, 2) - 1, 1, 2 * Random.Range(0, 2) - 1);
                    o.transform.localScale = randomScale;
                }
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        internal static void ApplyRoundPosition()
        {
            var selection = Selection.gameObjects;

            foreach (GameObject o in selection)
            {
                Vector3 currentPosition = o.transform.localPosition;
                currentPosition = new(
                    Mathf.Round(currentPosition.x / SnapStep) * SnapStep,
                    IncludeYAxis ? Mathf.Round(currentPosition.y / SnapStep) * SnapStep : currentPosition.y,
                    Mathf.Round(currentPosition.z / SnapStep) * SnapStep
                );

                o.transform.localPosition = currentPosition;
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        [MenuItem("GameObject/Keetzap/Create Separator", false, 0)]
        private static void CreateSeparator()
        {
            _ = new GameObject("────────────────────────");
        }
    }
}
