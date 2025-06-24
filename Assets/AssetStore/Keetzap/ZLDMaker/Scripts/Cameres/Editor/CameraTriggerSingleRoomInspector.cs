using UnityEditor;

namespace Keetzap.ZeldaMaker
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CameraTriggerSingleRoom))]
    public class CameraTriggerSingleRoomInspector : CameraTriggerRoomBaseInspector
    {
        public override void OnInspectorGUI()
        {
            InitInspector();

            Section("MAIN OBJECTS", SectionMainObjects);
            Section("BLACK FRAME", SectionBlackFrame);
            Section("BLACK FRAME BEHAVIOUR", SectionBlackFrameBehaviour);

            EndInspector((CameraTriggerSingleRoom)target, "Camera Trigger Single Room asset");
        }
    }
}