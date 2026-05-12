using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace Keetzap.ZeldaMaker
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ListenerFromDestruction))]
    public class ListenerFromDestructionInspector : ListenerInspector
    {
        private ListenerFromDestruction listenerFromDestruction;

        private SerializedProperty destructibles;

        private ReorderableList listDestructibles;

        protected override void OnEnable()
        {
            base.OnEnable();

            listenerFromDestruction = (ListenerFromDestruction)target;

            destructibles = serializedObject.FindProperty("destructibles");

            listDestructibles = new ReorderableList(serializedObject, destructibles, true, false, true, true);
        }

        public override void OnInspectorGUI()
        {
            InitInspector();

            Section("COMMON PROPERTIES", SectionMainProperties);
            Section("DESTRUCTIBLES", SectionDestructibles);
            Section("FEEDBACKS", SectionFeedbacksProperties);

            EndInspector(listenerFromDestruction, "Listener from Destruction asset");
        }

        private void SectionDestructibles()
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(GUIContent.none, GUILayout.MaxWidth(10));
                listDestructibles.drawElementCallback = DrawListDestructiblesItems;
                listDestructibles.DoLayoutList();
            }
            EditorGUILayout.EndHorizontal();
        }

        void DrawListDestructiblesItems(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = listDestructibles.serializedProperty.GetArrayElementAtIndex(index);

            EditorGUI.LabelField(new Rect(rect.x + 10, rect.y, 100, EditorGUIUtility.singleLineHeight), "Destructible");
            EditorGUI.PropertyField(new Rect(rect.x + 100, rect.y, rect.max.x - 150, EditorGUIUtility.singleLineHeight),
                                    element.FindPropertyRelative("destructible"),
                                    GUIContent.none);
        }
    }
}