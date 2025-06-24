using UnityEngine;
using UnityEditor;
using Keetzap.Core;
using UnityEditorInternal;

namespace Keetzap.ZeldaMaker
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Interactable))]
    public class TriggerInspector : BaseEditor
    {
        private SerializedProperty listeners;
        protected SerializedProperty activeOnAwake;
        private SerializedProperty showGizmos;
        private SerializedProperty debugColor;
        private SerializedProperty opacity;
        private SerializedProperty showDependencyLine;
        private SerializedProperty dependencyLineColor;

        private ReorderableList list;

        protected virtual void OnEnable()
        {
            listeners = serializedObject.FindProperty(Trigger.Fields.Listeners);
            //activeOnAwake = serializedObject.FindProperty(Trigger.Fields.ActiveOnAwake);

            showGizmos = serializedObject.FindProperty(Trigger.Fields.ShowGizmos);
            debugColor = serializedObject.FindProperty(Trigger.Fields.DebugColor);
            opacity = serializedObject.FindProperty(Trigger.Fields.Opacity);
            showDependencyLine = serializedObject.FindProperty(Trigger.Fields.ShowDependencyLinew);
            dependencyLineColor = serializedObject.FindProperty(Trigger.Fields.DependencyLineColor);

            list = new ReorderableList(serializedObject, listeners, true, false, true, true);
        }

        protected void SectionListeners()
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(GUIContent.none, GUILayout.MaxWidth(10));
                list.drawElementCallback = DrawListItems;
                list.DoLayoutList();
            }
            EditorGUILayout.EndHorizontal();
        }

        void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);

            EditorGUI.LabelField(new Rect(rect.x + 10, rect.y, 50, EditorGUIUtility.singleLineHeight), "Listener");
            EditorGUI.PropertyField(new Rect(rect.x + 70, rect.y, rect.max.x - 130, EditorGUIUtility.singleLineHeight),
                                    element.FindPropertyRelative("listener"),
                                    GUIContent.none);
        }

        protected void SectionDebug()
        {
            EditorGUILayout.PropertyField(showGizmos);
            GUILayout.Space(2);
            if (showGizmos.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(debugColor, new GUIContent("Color"));
                EditorGUILayout.Slider(opacity, 0, 1);
                EditorGUI.indentLevel--;
            }
            GUILayout.Space(2);

            EditorGUILayout.PropertyField(showDependencyLine);
            GUILayout.Space(2);
            if (showDependencyLine.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(dependencyLineColor, new GUIContent("Color"));
                EditorGUI.indentLevel--;
            }
        }
    }
}