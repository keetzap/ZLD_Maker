using UnityEditor;

namespace Keetzap.ZeldaMaker
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(InteractableChest))]
    public class InteractableChestInspector : InteractableTimelineInspector
    {
        private InteractableChest chest;

        private SerializedProperty drop;
        private SerializedProperty defaultGettingItemPosition;
        private SerializedProperty dropAnchorPosition;

        protected override void OnEnable()
        {
            base.OnEnable();

            chest = (InteractableChest)target;

            drop = serializedObject.FindProperty(InteractableChest.Fields.Drop);
            defaultGettingItemPosition = serializedObject.FindProperty(InteractableChest.Fields.DefaultGettingItemPosition);
            dropAnchorPosition = serializedObject.FindProperty(InteractableChest.Fields.DropAnchorPosition);
        }

        public override void OnInspectorGUI()
        {
            InitInspector();

            Section("COMMON PROPERTIES", SectionCommonProperties);
            Section("CHEST PROPERTIES", SectionChestProperties);
            Section("CHEST TIMELINE PROPERTIES", SectionTimelineProperties);
            Section("CHEST TIMELINE FEEDBACK", SectionInteractableTimelineFeedback);
            Section("DEBUG", SectionDebug);

            EndInspector(chest, "Interactable Chest Asset");
        }

        private void SectionChestProperties()
        {
            EditorGUILayout.PropertyField(drop);
            EditorGUILayout.PropertyField(dropAnchorPosition);
            EditorGUILayout.PropertyField(defaultGettingItemPosition);
        }
    }
}