using UnityEditor;

namespace Keetzap.Feedback
{
    [CustomEditor(typeof(PlayTimeline))]
    public class PlayTimelineInspector : FeedbackEffectEditor
    {
        private PlayTimeline playTimeline;

        private SerializedProperty timeline;

        protected override void OnEnable()
        {
            base.OnEnable();

            timeline = serializedObject.FindProperty(PlayTimeline.Fields.PlayableDirector);

            playTimeline = (PlayTimeline)target;
        }

        public override void OnInspectorGUI()
        {
            InitInspector();

            Section("COMMON SETTINGS", SectionCommonSettings);
            Section("PLAY TIMELINE SETTINGS", SectionProperties);

            EndInspector(playTimeline, "Play Timeline asset");
        }

        private void SectionProperties()
        {
            EditorGUILayout.PropertyField(timeline);

            playTimeline.SetTimelineDuration();
        }
    }
}