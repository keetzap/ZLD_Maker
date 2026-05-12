using UnityEngine;
using UnityEditor;
using Keetzap.Core;

namespace Keetzap.Feedback
{
    [CustomEditor(typeof(ImpulseObject))]
    public class ImpulseObjectInspector : FeedbackEffectEditor
    {
        private SerializedProperty model;
        private SerializedProperty typeOfImpulse;
        private SerializedProperty impulseLinearForce;
        private SerializedProperty impulseNegativeForce;
        private SerializedProperty impulseCurveForce;
        private SerializedProperty curveForce;

        protected override void OnEnable()
        {
            base.OnEnable();

            model = serializedObject.FindProperty(ImpulseObject.Fields.Model);
            typeOfImpulse = serializedObject.FindProperty(ImpulseObject.Fields.TypeOfImpulse);
            impulseLinearForce = serializedObject.FindProperty(ImpulseObject.Fields.ImpulseLinearForce);
            impulseNegativeForce = serializedObject.FindProperty(ImpulseObject.Fields.NegativeForce);
            impulseCurveForce = serializedObject.FindProperty(ImpulseObject.Fields.ImpulseCurveForce);
            curveForce = serializedObject.FindProperty(ImpulseObject.Fields.CurveForce);
        }

        public override void OnInspectorGUI()
        {
            InitInspector();

            Section("COMMON SETTINGS", SectionCommonSettings);
            Section("IMPULSE OBJECT", SectionBasicProperties);
            EditorGUI.EndChangeCheck();

            EndInspector((ImpulseObject)target, "Impulse Object asset");
        }

        private void SectionBasicProperties()
        {
            EditorGUILayout.PropertyField(model);
            EditorGUILayout.Space(2);
            if (model.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox(ASSIGN_MODEL, MessageType.Warning, true);
            }
            EditorGUILayout.Space(3);
            Decorators.SeparatorSimple();
            EditorGUILayout.Space(3);
            EditorGUILayout.PropertyField(typeOfImpulse);
            EditorGUILayout.Space(3);
            if (typeOfImpulse.enumValueIndex == 0)
            {
                EditorGUILayout.PropertyField(impulseLinearForce, new GUIContent("Impulse Force"));
                EditorGUILayout.PropertyField(impulseNegativeForce, new GUIContent("Negative Force"));
                impulseNegativeForce.floatValue = Mathf.Clamp(impulseNegativeForce.floatValue, -impulseLinearForce.floatValue, 0);
            }
            else
            {
                EditorGUILayout.PropertyField(impulseCurveForce, new GUIContent("Impulse Force"));
                EditorGUILayout.PropertyField(curveForce, new GUIContent("Animation curve"));
            }
        }
    }
}