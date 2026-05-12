using Keetzap.Core;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Keetzap.Feedback
{
    [CustomEditor(typeof(Blink))]
    public class BlinkInspector : FeedbackEffectEditor
    {
        private Blink blink;

        private SerializedProperty useThisObject;
        private SerializedProperty renderObject;
        private SerializedProperty sequence;

        private ReorderableList list;

        protected override void OnEnable()
        {
            base.OnEnable();

            blink = (Blink)target;

            useThisObject = serializedObject.FindProperty(Blink.Fields.UseThisObject);
            renderObject = serializedObject.FindProperty(Blink.Fields.RenderObject);
            sequence = serializedObject.FindProperty(Blink.Fields.Sequence);

            list = new ReorderableList(serializedObject, sequence, true, false, true, true);
            list.drawElementCallback = DrawListItems;
        }

        public override void OnInspectorGUI()
        {
            InitInspector();

            Section("COMMON SETTINGS", SectionCommonSettings);
            Section("BLINK SETTINGS", SectionProperties);

            EndInspector(blink, "Blink asset");
        }

        private void SectionProperties()
        {
            EditorGUIUtility.labelWidth = 180;
            EditorGUILayout.PropertyField(useThisObject, new GUIContent("Use this object as renderer"));
            EditorGUI.BeginDisabledGroup(useThisObject.boolValue);
            {
                
                EditorGUILayout.PropertyField(renderObject);
                EditorGUILayout.Space(2);
                if (renderObject.objectReferenceValue == null && !useThisObject.boolValue)
                {
                    EditorGUILayout.HelpBox(ASSIGN_MODEL, MessageType.Warning, true);
                }
            }
            
            EditorGUI.EndDisabledGroup();
            ResetLabelWidth();

            EditorGUILayout.Space(3);
            Decorators.SeparatorSimple();
            Decorators.HeaderBig("Blink sequence");
            EditorGUILayout.Space(3);
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(GUIContent.none, GUILayout.MaxWidth(12));
                list.DoLayoutList();
            }
            EditorGUILayout.EndHorizontal();

            blink.SetBlinkSequenceDuration();
        }

        private void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
        {
            float height = EditorGUIUtility.singleLineHeight;

            SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);
            EditorGUI.LabelField(new Rect(rect.x, rect.y, 70, height), new GUIContent("Duration"));
            EditorGUI.PropertyField(new Rect(rect.x + 70, rect.y, 40, height), element.FindPropertyRelative("blinkDuration"), GUIContent.none);
            EditorGUI.LabelField(new Rect(rect.max.x / 2, rect.y, 80, height), new GUIContent("Frequency"));
            EditorGUI.PropertyField(new Rect(rect.max.x / 2 + 80, rect.y, 40, height), element.FindPropertyRelative("blinkFrequency"), GUIContent.none);
        }
    }
}