using UnityEditor;

namespace Keetzap.Feedback
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SpawnObject))]
    public class SpawnObjectInspector : FeedbackEffectEditor
    {
        private SerializedProperty objectToSpawn;
        private SerializedProperty effect;

        protected override void OnEnable()
        {
            base.OnEnable();

            objectToSpawn = serializedObject.FindProperty(SpawnObject.Fields.ObjectToSpawn);
            effect = serializedObject.FindProperty(SpawnObject.Fields.Effect);
        }

        public override void OnInspectorGUI()
        {
            InitInspector();

            Section("COMMON SETTINGS", SectionCommonSettings);
            Section("SPAWN OBJECT", SectionProperties);

            EndInspector((SpawnObject)target, "Spawn Object asset");
        }

        private void SectionProperties()
        {
            EditorGUILayout.PropertyField(objectToSpawn);
            EditorGUILayout.PropertyField(effect);
        }
    }
}