using UnityEngine;
using UnityEditor;
/*
namespace Keetzap.Feedback
{
    [CustomEditor(typeof(CameraShake))]
    public class CameraShakeInspector : FeedbackEffectEditor
    {
        private SerializedProperty typeOfAmplitudeValue;
        private SerializedProperty amplitudeValue;
        private SerializedProperty amplitudeCurve;
        private SerializedProperty typeOfFrequenceValue;
        private SerializedProperty frequencyValue;
        private SerializedProperty frequencyCurve;

        protected override void OnEnable()
        {
            base.OnEnable();

            typeOfAmplitudeValue = serializedObject.FindProperty(CameraShake.Fields.TypeOfAmplitudeValue);
            amplitudeValue = serializedObject.FindProperty(CameraShake.Fields.AmplitudeValue);
            amplitudeCurve = serializedObject.FindProperty(CameraShake.Fields.AmplitudeCurve);
            typeOfFrequenceValue = serializedObject.FindProperty(CameraShake.Fields.TypeOfFrequenceValue);
            frequencyValue = serializedObject.FindProperty(CameraShake.Fields.FrequencyValue);
            frequencyCurve = serializedObject.FindProperty(CameraShake.Fields.FrequencyCurve);
        }

        public override void OnInspectorGUI()
        {
            InitInspector();

            Section("COMMON SETTINGS", SectionCommonSettings);
            Section("CAMERA SHAKE", SectionProperties);

            EndInspector((CameraShake)target, "Camera Shake asset");
        }

        private void SectionProperties()
        {
            EditorGUILayout.PropertyField(typeOfAmplitudeValue, new GUIContent("Amplitude"));
            EditorGUI.indentLevel++;
            if (typeOfAmplitudeValue.enumValueIndex == 0)
            {
                EditorGUILayout.PropertyField(amplitudeValue, new GUIContent("Value"));
            }
            else
            {
                EditorGUILayout.PropertyField(amplitudeCurve, new GUIContent("Curve"));
            }
            EditorGUI.indentLevel--;

            EditorGUILayout.Space(3);
            EditorGUILayout.PropertyField(typeOfFrequenceValue, new GUIContent("Frequence"));
            EditorGUI.indentLevel++;
            if (typeOfFrequenceValue.enumValueIndex == 0)
            {
                EditorGUILayout.PropertyField(frequencyValue, new GUIContent("Value"));
            }
            else
            {
                EditorGUILayout.PropertyField(frequencyCurve, new GUIContent("Curve"));
            }
            EditorGUI.indentLevel--;
        }
    }
}*/