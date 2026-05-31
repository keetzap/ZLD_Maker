using System;
using UnityEditor;
using UnityEngine;

namespace Keetzap.EditorTools
{
    /// <summary>
    /// Custom inspector for Transform that appends shortcut buttons
    /// for SceneUtilities operations. Toggled via the Keetzap menu.
    /// </summary>
    [CustomEditor(typeof(Transform), true)]
    [CanEditMultipleObjects]
    public class TransformCustomInspector : Editor
    {
        private const string MenuPath = "Keetzap/Show Transform Utilities";
        private const string PrefKey = "Keetzap_ShowTransformUtilities";
        private const float ButtonHeight = 22f;

        private static bool _showUtilities;
        private Editor _defaultEditor;

        [InitializeOnLoadMethod]
        private static void InitMenu()
        {
            EditorApplication.delayCall += () =>
            {
                _showUtilities = EditorPrefs.GetBool(PrefKey, false);
                Menu.SetChecked(MenuPath, _showUtilities);
            };
        }

        [MenuItem(MenuPath, false, 200)]
        private static void ToggleShowUtilities()
        {
            _showUtilities = !_showUtilities;
            EditorPrefs.SetBool(PrefKey, _showUtilities);
            Menu.SetChecked(MenuPath, _showUtilities);

            // Force all Transform inspectors to repaint.
            foreach (var editor in ActiveEditorTracker.sharedTracker.activeEditors)
            {
                if (editor.target is Transform)
                    editor.Repaint();
            }
        }

        private void OnEnable()
        {
            Type defaultType = Type.GetType("UnityEditor.TransformInspector, UnityEditor");
            _defaultEditor = CreateEditor(targets, defaultType);
        }

        private void OnDisable()
        {
            DestroyImmediate(_defaultEditor);
        }

        public override void OnInspectorGUI()
        {
            _defaultEditor.OnInspectorGUI();

            if (!_showUtilities)
                return;

            EditorGUILayout.Space(4);
            Decorators.Separator();

            EditorGUILayout.BeginHorizontal();
            {
                // Round Position button (first)
                if (GUILayout.Button("Round Position", GUILayout.Height(ButtonHeight)))
                {
                    SceneUtilities.ApplyRoundPosition();
                }

                // Rotate +90 on Y axis
                if (GUILayout.Button("Rotate +90", GUILayout.Height(ButtonHeight)))
                {
                    foreach (Transform t in targets)
                    {
                        Undo.RecordObject(t, "Rotate +90 Y");
                        Vector3 euler = t.eulerAngles;
                        euler.y += 90f;
                        t.eulerAngles = euler;
                    }
                }

                // Rotate -90 on Y axis
                if (GUILayout.Button("Rotate -90", GUILayout.Height(ButtonHeight)))
                {
                    foreach (Transform t in targets)
                    {
                        Undo.RecordObject(t, "Rotate -90 Y");
                        Vector3 euler = t.eulerAngles;
                        euler.y -= 90f;
                        t.eulerAngles = euler;
                    }
                }

                // Add Random Transformation button (last)
                if (GUILayout.Button("Random Transf", GUILayout.Height(ButtonHeight)))
                {
                    SceneUtilities.ApplyTransformationsOnly();
                }
                // Reset Transform button
                if (GUILayout.Button("Reset", GUILayout.Height(ButtonHeight)))
                {
                    foreach (Transform t in targets)
                    {
                        Undo.RecordObject(t, "Reset Transform");
                        t.position = Vector3.zero;
                        t.rotation = Quaternion.identity;
                        t.localScale = Vector3.one;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
