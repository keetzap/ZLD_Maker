using UnityEditor;
using UnityEngine;

namespace Keetzap.Core
{
    public class BaseEditor : Editor
    {
        protected readonly string NO_PROPERTIES = "This inspector doesn't have any property to show.";

        public delegate void DrawSection();
        protected float labelWidth;
        private static GUIStyle _foldoutStyle = new();

        private void OnEnable()
        {
            labelWidth = EditorGUIUtility.labelWidth;

            if (EditorStyles.foldout != null)
            {
                _foldoutStyle = new GUIStyle(EditorStyles.foldout)
                {
                    fontStyle = FontStyle.Bold,
                };
            }
        }

        protected void InitInspector()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
        }

        protected void EndInspector(Object @object, string registerLabel)
        {
            if (EditorGUI.EndChangeCheck())
                Undo.RegisterCompleteObjectUndo(@object, registerLabel);

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
                EditorUtility.SetDirty(@object);
        }

        public virtual void Section(DrawSection onDrawSection)
        {
            HeaderWithoutTitle();
            onDrawSection();
            Footer();
        }

        public virtual void Section(string title, DrawSection onDrawSection)
        {
            HeaderWithTitle(title);
            onDrawSection();
            Footer();
        }

        public virtual void SectionFoldout(string title, ref SerializedProperty serializedProperty, DrawSection onDrawSection)
        {
            HeaderBegin();

            GUI.color = Color.red;
            string header = string.Format($" {title}");
            serializedProperty.isExpanded = EditorGUILayout.Foldout(serializedProperty.isExpanded, new GUIContent(header), true, _foldoutStyle);
            GUI.color = Color.white;
            Decorators.Separator();

            HeaderEnd();

            if (serializedProperty.isExpanded)
            {
                GUI.color = Color.white;
                onDrawSection();
            }

            Footer();
        }

        private void HeaderWithoutTitle()
        {
            HeaderBegin();
            HeaderEnd();
        }

        private void HeaderWithTitle(string title)
        {
            HeaderBegin();
            GUI.color = Color.grey;
            string header = string.Format($" {title}");
            EditorGUILayout.LabelField(header, EditorStyles.boldLabel);
            GUI.color = Color.white;
            Decorators.Separator();
            HeaderEnd();
        }

        private void HeaderBegin()
        {
            GUI.color = new Color(0.75f, 0.75f, 0.75f);
            EditorGUILayout.BeginVertical("Box");
            GUILayout.Space(1);
        }

        private void HeaderEnd()
        {
            GUI.color = Color.white;
            GUILayout.Space(1);
            EditorGUI.indentLevel++;
        }

        private void Footer()
        {
            EditorGUILayout.LabelField(GUIContent.none, GUILayout.Height(5));
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }

        protected void MinMaxSliderValues(float initSpace, string label, float widthLabel, ref float minVal, float widthMinVal, ref float maxVal, float widthMaxVal, float minLimit, float maxLimit)
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("", GUILayout.MinWidth(initSpace));
                GUILayout.Label(label, GUILayout.MaxWidth(widthLabel));
                minVal = EditorGUILayout.FloatField(minVal, GUILayout.MaxWidth(widthMinVal));
                EditorGUILayout.MinMaxSlider(ref minVal, ref maxVal, minLimit, maxLimit);
                maxVal = EditorGUILayout.FloatField(maxVal, GUILayout.MaxWidth(widthMaxVal));
            }
            EditorGUILayout.EndHorizontal();
        }

        protected void SetLabelWidth(float width) => EditorGUIUtility.labelWidth = width;

        protected void ResetLabelWidth() =>  EditorGUIUtility.labelWidth = labelWidth;
    }
}