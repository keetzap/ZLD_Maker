using UnityEditor;
using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(InteractableFountain))]
    public class InteractableFountainInspector : InteractableTimelineInspector
    {
        private InteractableFountain fountain;

        private SerializedProperty refillCompletely;
        private SerializedProperty numberOfLifes;

        protected override void OnEnable()
        {
            base.OnEnable();

            fountain = (InteractableFountain)target;

            refillCompletely = serializedObject.FindProperty(InteractableFountain.Fields.RefillCompletely);
            numberOfLifes = serializedObject.FindProperty(InteractableFountain.Fields.NumberOfLifes);
        }

        public override void OnInspectorGUI()
        {
            InitInspector();

            Section("COMMON PROPERTIES", SectionCommonProperties);
            Section("FOUNTAIN PROPERTIES", SectionFountainProperties);
            Section("FOUNTAIN TIMELINE PROPERTIES", SectionTimelineProperties);
            Section("FOUNTAIN TIMELINE FEEDBACK", SectionInteractableTimelineFeedback);
            Section("DEBUG", SectionDebug);

            EndInspector(fountain, "Interactable Fountain Asset");
        }

        private void SectionFountainProperties()
        {
            EditorGUILayout.PropertyField(refillCompletely, new GUIContent("Refill all Lifes"));
            EditorGUI.BeginDisabledGroup(refillCompletely.boolValue == true);
            {
                EditorGUILayout.PropertyField(numberOfLifes);
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}