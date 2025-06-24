using UnityEngine;
using UnityEditor;
using Keetzap.Core;

namespace Keetzap.Feedback
{
    [CustomEditor(typeof(SimpleFlicker))]
    public class SimpleFlickerInspector : FeedbackEffectEditor
    {
        private SerializedProperty renderObject;
        private SerializedProperty flickerFrequence;
        private SerializedProperty colorReplacement;
        private SerializedProperty flickerColor;
        private SerializedProperty flickerMaterial;

        protected override void OnEnable()
        {
            base.OnEnable();

            renderObject = serializedObject.FindProperty(SimpleFlicker.Fields.RenderObject);
            flickerFrequence = serializedObject.FindProperty(SimpleFlicker.Fields.FlickerFrequence);
            colorReplacement = serializedObject.FindProperty(SimpleFlicker.Fields.ColorReplacement);
            flickerColor = serializedObject.FindProperty(SimpleFlicker.Fields.FlickerColor);
            flickerMaterial = serializedObject.FindProperty(SimpleFlicker.Fields.FlickerMaterial);
        }

        public override void OnInspectorGUI()
        {
            InitInspector();

            Section("COMMON SETTINGS", SectionCommonSettings);
            Section("SIMPLE FLICKER", SectionProperties);
            EditorGUI.EndChangeCheck();

            EndInspector((SimpleFlicker)target, "Simple Flicker asset");
        }

        private void SectionProperties()
        {
            EditorGUILayout.PropertyField(renderObject);
            EditorGUILayout.Space(2);
            Decorators.SeparatorSimple();
            EditorGUILayout.Space(2);
            EditorGUILayout.PropertyField(flickerFrequence);
            EditorGUILayout.PropertyField(colorReplacement, new GUIContent("Replacement Mode"));
            
            if (colorReplacement.enumValueIndex == 0)
            {
                EditorGUILayout.PropertyField(flickerColor);
            }
            else
            {
                EditorGUILayout.PropertyField(flickerMaterial);
            }
        }
    }
}