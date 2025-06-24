using Keetzap.Core;
using UnityEditor;
using UnityEngine;

namespace Keetzap.Feedback
{
    [CustomEditor(typeof(FeedbackEffect), true, isFallback = true)]
    public class FeedbackEffectEditor : BaseEditor
    {
        protected readonly string ASSIGN_MODEL = "In order to play this feeback, a model must be assigned.";

        private SerializedProperty label;
        private SerializedProperty delay;
        private SerializedProperty duration;
        private SerializedProperty childControlsDuration;
        private SerializedProperty ignoreStopCoroutine;

        protected virtual void OnEnable()
        {
            label = serializedObject.FindProperty(FeedbackEffect.Fields.DisplayLabel);
            delay = serializedObject.FindProperty(FeedbackEffect.Fields.Delay);
            duration = serializedObject.FindProperty(FeedbackEffect.Fields.Duration);
            childControlsDuration = serializedObject.FindProperty(FeedbackEffect.Fields.ChildControlsDuration);
            ignoreStopCoroutine = serializedObject.FindProperty(FeedbackEffect.Fields.IgnoreStopCoroutine);

            ((MonoBehaviour)target).hideFlags = HideFlags.HideInInspector;
        }

        protected void SectionCommonSettings()
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(label, new GUIContent("Label"));
            EditorGUILayout.PropertyField(delay);
            delay.floatValue = Mathf.Clamp(delay.floatValue, 0, Mathf.Infinity);

            var controlDuration = childControlsDuration.FindPropertyRelative("childControlsDuration").boolValue;
            var message = childControlsDuration.FindPropertyRelative("message").stringValue;
             
            EditorGUI.BeginDisabledGroup(controlDuration);
            {
                EditorGUILayout.PropertyField(duration);
                duration.floatValue = Mathf.Clamp(duration.floatValue, 0, Mathf.Infinity);
            }
            EditorGUI.EndDisabledGroup();

            if (controlDuration && !string.IsNullOrEmpty(message))
            {
                EditorGUILayout.HelpBox(message, MessageType.Info, true);
            }

            Decorators.SeparatorSimple();
            EditorGUILayout.PropertyField(ignoreStopCoroutine, new GUIContent("Ignore stop coroutines"));
            EditorGUILayout.Space(2);
            if (ignoreStopCoroutine.boolValue)
            {
                EditorGUILayout.HelpBox("Having this option enabled might cause unexpected errors. " +
                    "Be sure the effect you're not stopping doesn't affect the source object.", MessageType.Warning, true);
            }
        }
    }
}