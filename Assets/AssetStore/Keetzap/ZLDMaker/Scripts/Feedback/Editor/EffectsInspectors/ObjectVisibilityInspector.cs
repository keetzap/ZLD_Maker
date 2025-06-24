using UnityEditor;

namespace Keetzap.Feedback
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ObjectVisibility))]
    public class ObjectVisibilityInspector : FeedbackEffectEditor
    {
        private SerializedProperty objectToEnable;
        private SerializedProperty visibility;

        protected override void OnEnable()
        {
            base.OnEnable();

            objectToEnable = serializedObject.FindProperty(ObjectVisibility.Fields.ObjectToEnable);
            visibility = serializedObject.FindProperty(ObjectVisibility.Fields.Visibility);
        }

        public override void OnInspectorGUI()
        {
            InitInspector();

            Section("COMMON SETTINGS", SectionCommonSettings);
            Section("OBJECT VISIBILITY", SectionProperties);

            EndInspector((ObjectVisibility)target, "Object Visibility asset");
        }

        private void SectionProperties()
        {
            EditorGUILayout.PropertyField(objectToEnable);
            EditorGUILayout.PropertyField(visibility);
        }
    }
}