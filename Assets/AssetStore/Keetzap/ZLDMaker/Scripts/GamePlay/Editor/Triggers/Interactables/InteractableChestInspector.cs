using UnityEditor;

namespace Keetzap.ZeldaMaker
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(InteractableChest))]
    public class InteractableChestInspector : InteractableTimelineInspector
    {
        private InteractableChest chest;

        private SerializedProperty drop;
        private SerializedProperty gettingItemPosition;
        private SerializedProperty collectableAnchorPosition;

        protected override void OnEnable()
        {
            base.OnEnable();

            chest = (InteractableChest)target;

            drop = serializedObject.FindProperty(InteractableChest.Fields.Drop);
            gettingItemPosition = serializedObject.FindProperty(InteractableChest.Fields.GettingItemPosition);
            collectableAnchorPosition = serializedObject.FindProperty(InteractableChest.Fields.CollectableAnchorPosition);
        }

        public override void OnInspectorGUI()
        {
            InitInspector();

            Section("COMMON PROPERTIES", SectionCommonProperties);
            Section("CHEST PROPERTIES", SectionChestProperties);
            Section("TIMELINE PROPERTIES", SectionTimelineProperties);
            Section("FEEDBACKS", SectionInteractableTimelineFeedback);
            Section("DEBUG", SectionDebug);

            EndInspector(chest, "Interactable Chest Asset");
        }

        private void SectionChestProperties()
        {
            EditorGUILayout.PropertyField(drop);
            EditorGUILayout.PropertyField(collectableAnchorPosition);
            EditorGUILayout.PropertyField(gettingItemPosition);
        }
    }
}