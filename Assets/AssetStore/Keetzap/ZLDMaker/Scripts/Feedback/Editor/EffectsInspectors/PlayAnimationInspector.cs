using UnityEditor;

namespace Keetzap.Feedback
{
    [CustomEditor(typeof(PlayAnimation))]
    public class PlayAnimationInspector : FeedbackEffectEditor
    {
        private SerializedProperty animator;
        private SerializedProperty stateName;

        protected override void OnEnable()
        {
            base.OnEnable();

            animator = serializedObject.FindProperty(PlayAnimation.Fields.Animator);
            stateName = serializedObject.FindProperty(PlayAnimation.Fields.StateName);
        }

        public override void OnInspectorGUI()
        {
            InitInspector();

            Section("COMMON SETTINGS", SectionCommonSettings);
            Section("ANIMATION SETTINGS", SectionProperties);

            EndInspector((PlayAnimation)target, "Play animation asset");
        }

        private void SectionProperties()
        {
            EditorGUILayout.PropertyField(animator);
            EditorGUILayout.PropertyField(stateName);
        }
    }
}