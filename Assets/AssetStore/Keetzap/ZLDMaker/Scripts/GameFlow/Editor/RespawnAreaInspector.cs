using UnityEditor;
using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    [CustomEditor(typeof(RespawnArea))]
    public class RespawnAreaInspector : RespawnSystemInspector
    {
        private RespawnArea respawnArea;

        private SerializedProperty lifeCost;
        private SerializedProperty timeToRespawn;

        protected override void OnEnable()
        {
            base.OnEnable();

            respawnArea = (RespawnArea)target;

            lifeCost = serializedObject.FindProperty(RespawnArea.Fields.LifeCost);
            timeToRespawn = serializedObject.FindProperty(RespawnArea.Fields.TimeToRespawn);
        }

        public override void OnInspectorGUI()
        {
            InitInspector();

            Section("RESPAWN PROPERTIES", SectionRespawnProperties);
            Section("RESPAWNING POINTS", SectionRespawningPoints);

            EndInspector(respawnArea, "Respawning area asset");
        }

        private void SectionRespawnProperties()
        {
            EditorGUILayout.IntSlider(lifeCost, 0, -10, new GUIContent("Life Cost"));
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(timeToRespawn, new GUIContent("Time to Respawn (s)"));
        }
    }
}