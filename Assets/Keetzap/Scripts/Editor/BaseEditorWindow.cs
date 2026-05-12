using Codice.CM.Triggers;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEditor.VersionControl;
using UnityEngine;
using vietlabs.fr2;

namespace Keetzap.EditorTools
{
    public class BaseEditorWindow : EditorWindow
    {
        private const int Year = 2025;
        private const string Keetzap = "Keetzap";
        private const float HelpHeight = 30;
        private const float SizeThreshold = 0.01f;

        private float _buttonHeight = 26;
        protected static Vector4 margings = new (5, 5, 5, 5);
        protected float labelWidth;
        protected string helpMessage;
        protected float ButtonHeight => _buttonHeight;

        private void OnEnable()
        {
            labelWidth = EditorGUIUtility.labelWidth;
        }

        void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("", GUILayout.MaxWidth(margings.w));
                EditorGUILayout.BeginVertical();
                {
                    EditorGUILayout.Space(margings.x);
                    MainSection();
                    EditorGUILayout.Space(margings.z);
                    DrawFooter(helpMessage);
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.LabelField("", GUILayout.MaxWidth(margings.y));
            }
            EditorGUILayout.EndHorizontal();
        }

        protected virtual void MainSection() { }

        protected virtual void DrawFooter(string helpMessage)
        {
            EditorGUILayout.Space(5);
            Decorators.Separator();
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();

                GUI.enabled = false;
                GUILayout.Label($"{Keetzap} {Year}", EditorStyles.miniLabel);
                GUI.enabled = true;

                GUIContent icon = EditorGUIUtility.IconContent("_Help");
                icon.tooltip = "Show information about this tool";
                if (GUILayout.Button(icon, GUIStyle.none, GUILayout.Width(20), GUILayout.Height(20)))
                {
                    EditorUtility.DisplayDialog("Help", helpMessage, "Close");
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        protected void SetLabelWidth(float width) => EditorGUIUtility.labelWidth = width;

        protected void ResetLabelWidth() => EditorGUIUtility.labelWidth = labelWidth;

        static protected void ShowWindow(EditorWindow window, string name, float width, float height, bool help)
        {
            window.minSize = new Vector2(width, height + (help ? HelpHeight : 0));
            window.maxSize = window.minSize;
            window.titleContent.text = name;
            window.Show();
        }

        static protected void ShowWindow(EditorWindow window, string name, float top, float right, float bottom, float left, float width, float height, bool help)
        {
            SetMargings(top, right, bottom, left);
            window.minSize = new Vector2(width + right + left, height + top + bottom + (help ? HelpHeight : 0));
            window.maxSize = new Vector2(SizeThreshold + width + right + left, height + top + bottom + (help ? HelpHeight : 0));
            window.titleContent.text = name;
            window.Show();

        }

        static protected void ShowWindow(EditorWindow window, string name, float top, float right, float bottom, float left, float minWidth, float maxWidth, float height, bool help)
        {
            SetMargings(top, right, bottom, left);
            window.minSize = new Vector2(minWidth + right + left, height + top + bottom + (help ? HelpHeight : 0));
            window.maxSize = new Vector2(SizeThreshold + maxWidth + right + left, height + top + bottom + (help ? HelpHeight : 0));
            window.titleContent.text = name;
            window.Show();
        }

        static protected void ShowWindow(EditorWindow window, string name, float top, float right, float bottom, float left, float minWidth, float maxWidth, float minHeight, float maxHeight, bool help)
        {
            SetMargings(top, right, bottom, left);
            window.minSize = new Vector2(minWidth + right + left, minHeight + top + bottom + (help ? 30 : 0));
            window.maxSize = new Vector2(SizeThreshold + maxWidth + right + left, SizeThreshold + maxHeight + top + bottom + (help ? 30 : 0));
            window.titleContent.text = name;
            window.Show();
        }

        static protected void SetMargings(float top, float right, float bottom, float left)
        {
            margings = new Vector4(top, right, bottom, left);
        }

        static protected void SetSize(EditorWindow window, float minWidth, float maxWidth, float minHeight, float maxHeight)
        {
            window.minSize = new Vector2(minWidth + margings.y + margings.w, minHeight + margings.x + margings.z);
            window.maxSize = new Vector2(maxWidth + margings.y + margings.w, maxHeight + margings.x + margings.z);
        }
    }
}
