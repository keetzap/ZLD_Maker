using UnityEngine;
using UnityEditor;
using Keetzap.Core;

namespace Keetzap.ZeldaMaker
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Hitable))]
    public class HitableInspector : BaseEditor
    {
        private Hitable hitable;

        private SerializedProperty configurationFile;
        private SerializedProperty model;
        private SerializedProperty damageFeedback;
        private SerializedProperty deathFeedback;
        private SerializedProperty destroyObject;
        private SerializedProperty typeOfDestruction;
        private SerializedProperty delay;

        void OnEnable()
        {
            hitable = (Hitable)target;

            configurationFile = serializedObject.FindProperty(Hitable.Fields.ConfigurationFile);
            model = serializedObject.FindProperty(Hitable.Fields.Model);
            damageFeedback = serializedObject.FindProperty(Hitable.Fields.DamageFeedback);
            deathFeedback = serializedObject.FindProperty(Hitable.Fields.DeathFeedback);
            destroyObject = serializedObject.FindProperty(Hitable.Fields.DestroyObject);
            typeOfDestruction = serializedObject.FindProperty(Hitable.Fields.Destruction);
            delay = serializedObject.FindProperty(Hitable.Fields.Delay);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            Section("HITABLE PROPERTIES", SectionProperties);
            Section("HITABLE FEEDBACKS", SectionFeedbacks);

            if (EditorGUI.EndChangeCheck())
                Undo.RegisterCompleteObjectUndo(hitable, "Hitable");

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
                EditorUtility.SetDirty(hitable);
        }

        private void SectionProperties()
        {
            EditorGUILayout.PropertyField(configurationFile);
            EditorGUILayout.PropertyField(model);

            EditorGUI.BeginDisabledGroup(true);
            {
                string life = hitable.GameDataAsset() == null ? "-" : hitable.GameDataAsset().life.ToString();
                EditorGUILayout.LabelField($"HP: {life}");
            }
            EditorGUI.EndDisabledGroup();

            EditorGUIUtility.labelWidth = 200;

            Decorators.SeparatorSimple();
            EditorGUILayout.Space(2);
            EditorGUILayout.PropertyField(destroyObject);

            EditorGUI.BeginDisabledGroup(!destroyObject.boolValue);
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(typeOfDestruction, new GUIContent("Delay on destruction"));

                float delayFromFeedback = hitable.GetDelayFromFeedback();

                if (typeOfDestruction.enumValueIndex == (int)TypeOfDestruction.AfterFeedbackDuration)
                {
                    EditorGUI.BeginDisabledGroup(true);
                    {
                        EditorGUILayout.FloatField(new GUIContent("Feedback Effect Duration"), delayFromFeedback);
                    }
                    EditorGUI.EndDisabledGroup();
                }
                else if (typeOfDestruction.enumValueIndex == (int)TypeOfDestruction.AfterDelay)
                {
                    EditorGUILayout.PropertyField(delay);
                    delay.floatValue = Mathf.Clamp(delay.floatValue, 0, Mathf.Infinity);
                    if (delay.floatValue < delayFromFeedback)
                    {
                        EditorGUILayout.Space(2);
                        EditorGUILayout.HelpBox($"The value of 'Delay' is less than the duration of the Feedback Effect: {delayFromFeedback}", MessageType.Warning, true);
                    }
                }
                EditorGUI.indentLevel--;
            }

            ResetLabelWidth();
        }

        private void SectionFeedbacks()
        {
            EditorGUILayout.PropertyField(damageFeedback, new GUIContent("On Hit"));
            EditorGUILayout.Space(2);
            EditorGUILayout.PropertyField(deathFeedback, new GUIContent("On Death"));
        }
    }
}