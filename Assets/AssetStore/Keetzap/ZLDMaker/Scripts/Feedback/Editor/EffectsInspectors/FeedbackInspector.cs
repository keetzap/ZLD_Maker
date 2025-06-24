/*
using UnityEngine;
using UnityEditor;

namespace Keetzap.Feedback
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Feedback))]
    public class FeedbackInspector : BaseEditor
    {
        protected Feedback feedback;
        private SerializedProperty delay;

        protected void OnEnable()
        {
            feedback = (Feedback)target;

            delay = serializedObject.FindProperty("delay");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            Section("COMMON SETTINGS", SectionCommonSettings);

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(feedback);
            }
        }

        private void SectionCommonSettings()
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(delay);
            delay.floatValue = Mathf.Clamp(delay.floatValue, 0, Mathf.Infinity);
            feedback.delay = delay.floatValue;

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RegisterCompleteObjectUndo(feedback, "Feedback Delay");
            }
        }
    }
}


*/