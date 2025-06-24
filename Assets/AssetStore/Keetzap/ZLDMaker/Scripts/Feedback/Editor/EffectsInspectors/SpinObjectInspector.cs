using UnityEngine;
using UnityEditor;
using Keetzap.Core;

namespace Keetzap.Feedback
{
    [CustomEditor(typeof(SpinObject))]
    public class SpinObjectInspector : FeedbackEffectEditor
    {
        private SerializedProperty model;
        private SerializedProperty spinSpeed;
        private SerializedProperty spinAxis;
        private SerializedProperty returnInitRotation;
        private SerializedProperty useAccelDeceleration;
        private SerializedProperty spinAccelDeceleration;
        private SerializedProperty accelDecelValue;
        private SerializedProperty accelDecelCurve;

        protected override void OnEnable()
        {
            base.OnEnable();

            model = serializedObject.FindProperty(SpinObject.Fields.Model);
            spinSpeed = serializedObject.FindProperty(SpinObject.Fields.SpinSpeed);
            spinAxis = serializedObject.FindProperty(SpinObject.Fields.SpinAxis);
            returnInitRotation = serializedObject.FindProperty(SpinObject.Fields.ReturnInitRotation);
            useAccelDeceleration = serializedObject.FindProperty(SpinObject.Fields.UseAccelDeceleration);
            spinAccelDeceleration = serializedObject.FindProperty(SpinObject.Fields.SpinAccelDeceleration);
            accelDecelValue = serializedObject.FindProperty(SpinObject.Fields.AccelDecelValue);
            accelDecelCurve = serializedObject.FindProperty(SpinObject.Fields.AccelDecelCurve);
        }

        public override void OnInspectorGUI()
        {
            InitInspector();

            Section("COMMON SETTINGS", SectionCommonSettings);
            Section("BASIC SPIN SETTINGS", SectionBasicProperties);
            Section("ADVANCED SPIN SETTINGS", SectionAdvancedProperties);

            EndInspector((SpinObject)target, "Spin Object asset");
        }

        private void SectionBasicProperties()
        {
            EditorGUIUtility.labelWidth = 180;

            EditorGUILayout.PropertyField(model);
            EditorGUILayout.Space(2);
            if (model.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox(ASSIGN_MODEL, MessageType.Warning, true);
            }
            EditorGUILayout.Space(3);
            Decorators.SeparatorSimple();
            EditorGUILayout.PropertyField(spinSpeed, new GUIContent("Speed"));
            EditorGUILayout.PropertyField(spinAxis, new GUIContent("Axis"));
            EditorGUILayout.PropertyField(returnInitRotation, new GUIContent("Return to its initial rotation"));
            
            ResetLabelWidth();
        }

        private void SectionAdvancedProperties()
        {
            EditorGUILayout.PropertyField(useAccelDeceleration, new GUIContent("Use ease"));
            EditorGUILayout.Space(2);
            EditorGUI.BeginDisabledGroup(!useAccelDeceleration.boolValue);
            {
                EditorGUILayout.PropertyField(spinAccelDeceleration, new GUIContent("Type of ease"));
                EditorGUI.indentLevel++;
                if (spinAccelDeceleration.enumValueIndex == 0)
                {
                    EditorGUILayout.PropertyField(accelDecelValue, new GUIContent("Fix value"));
                }
                else
                {
                    EditorGUILayout.PropertyField(accelDecelCurve, new GUIContent("Animation curve"));
                }
                EditorGUI.indentLevel--;
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}