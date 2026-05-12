using UnityEngine;
using UnityEditor;
using Keetzap.Core;

namespace Keetzap.Feedback
{
    [CustomEditor(typeof(SoundEffect))]
    public class SoundEffectInspector : FeedbackEffectEditor
    {
        private SerializedProperty clip;
        private SerializedProperty playRandomSound;
        private SerializedProperty randomClips;
        private SerializedProperty loop;
        private SerializedProperty minVolume;
        private SerializedProperty maxVolume;
        private SerializedProperty minPitch;
        private SerializedProperty maxPitch;

        private float minLimit = 0;
        private float maxLimit = 2;
        float minV;
        float maxV;
        float minP;
        float maxP;

        protected override void OnEnable()
        {
            base.OnEnable();

            clip = serializedObject.FindProperty(SoundEffect.Fields.Clip);
            playRandomSound = serializedObject.FindProperty(SoundEffect.Fields.PlayRandomSound);
            randomClips = serializedObject.FindProperty(SoundEffect.Fields.RandomClips);
            loop = serializedObject.FindProperty(SoundEffect.Fields.Loop);
            minVolume = serializedObject.FindProperty(SoundEffect.Fields.MinVolume);
            maxVolume = serializedObject.FindProperty(SoundEffect.Fields.MaxVolume);
            minPitch = serializedObject.FindProperty(SoundEffect.Fields.MinPitch);
            maxPitch = serializedObject.FindProperty(SoundEffect.Fields.MaxPitch);

            minV = minVolume.floatValue;
            maxV = maxVolume.floatValue;
            minP = minPitch.floatValue;
            maxP = maxPitch.floatValue;
        }

        public override void OnInspectorGUI()
        {
            InitInspector();

            Section("COMMON SETTINGS", SectionCommonSettings);
            Section("SOUND EFFECT", SectionProperties);

            EndInspector((SoundEffect)target, "Sound Effect asset");
        }

        private void SectionProperties()
        {
            EditorGUILayout.PropertyField(clip);
            Decorators.SeparatorSimple();
            EditorGUILayout.Space(3);
            EditorGUILayout.PropertyField(playRandomSound);
            if (playRandomSound.boolValue)
            {
                EditorGUILayout.Space(3);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(randomClips);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.Space(3);
            Decorators.SeparatorSimple();
            Decorators.HeaderBig("Clip Settings");
            EditorGUILayout.Space(3);

            EditorGUI.indentLevel++;
            { 
                EditorGUIUtility.labelWidth = 95;
                EditorGUILayout.PropertyField(loop);
                ResetLabelWidth();
            }
            EditorGUI.indentLevel--;

            EditorGUILayout.Space(3);
            MinMaxSliderValues(23, "Volume:", 50, ref minV, 65, ref maxV, 65, minLimit, maxLimit);
            minVolume.floatValue = minV;
            maxVolume.floatValue = maxV;
            EditorGUILayout.Space(2);
            MinMaxSliderValues(24, "Pitch:", 55, ref minP, 65, ref maxP, 65, minLimit, maxLimit);
            minPitch.floatValue = minP;
            maxPitch.floatValue = maxP;
        }
    }
}