using UnityEditor;
/*
namespace Keetzap.ZeldaMaker
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CameraTriggerDolly), true)]
    public class CameraTriggerDollyInspector : CameraTriggerRoomBaseInspector
    {
        private SerializedProperty dollyTrack;
        private SerializedProperty startingPoint;

        protected override void OnEnable()
        {
            base.OnEnable();

            dollyTrack = serializedObject.FindProperty(CameraTriggerDolly.Fields.DollyTrack);
            startingPoint = serializedObject.FindProperty(CameraTriggerDolly.Fields.StartingPoint);
        }

        public override void OnInspectorGUI()
        {
            InitInspector();

            Section("MAIN OBJECTS", SectionMainObjects);
            Section("BLACK FRAME", SectionBlackFrame);
            Section("BLACK FRAME BEHAVIOUR", SectionBlackFrameBehaviour);
            Section("DOLLY TRACK PROPERTIES", SectionDollyTrackProperties);

            EndInspector((CameraTriggerDolly)target, "Dolly Camera Trigger asset");
        }

        private void SectionDollyTrackProperties()
        {
            EditorGUILayout.PropertyField(dollyTrack);
            EditorGUILayout.PropertyField(startingPoint);
        }
    }
}*/