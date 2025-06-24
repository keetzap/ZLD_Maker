using UnityEngine;
using UnityEditor;
using Keetzap.Core;

namespace Keetzap.ZeldaMaker
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(RespawnPoint))]
    public class RespawnPointInspector : BaseEditor
    {
        private RespawnPoint respawnPoint;

        private SerializedProperty showGizmos;
        private SerializedProperty opacity;
        private SerializedProperty spawnColor, spawnRadius;
        private SerializedProperty heightBox;
        private SerializedProperty arrowLength;

        void OnEnable()
        {
            respawnPoint = (RespawnPoint)target;

            showGizmos = serializedObject.FindProperty(RespawnPoint.Fields.ShowGizmos);
            opacity = serializedObject.FindProperty(RespawnPoint.Fields.Opacity);
            spawnColor = serializedObject.FindProperty(RespawnPoint.Fields.SpawnColor);
            spawnRadius = serializedObject.FindProperty(RespawnPoint.Fields.SpawnRadius);
            heightBox = serializedObject.FindProperty(RespawnPoint.Fields.HeightBox);
            arrowLength = serializedObject.FindProperty(RespawnPoint.Fields.ArrowLength);
        }

        public override void OnInspectorGUI()
        {
            InitInspector();

            Section("DEBUG", SectionDebug);

            EndInspector(respawnPoint, "Respawn Point");
        }

        private void SectionDebug()
        {
            EditorGUILayout.PropertyField(showGizmos);
            GUILayout.Space(2);

            if (showGizmos.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.Slider(opacity, 0, 1);
                EditorGUILayout.PropertyField(spawnColor, new GUIContent("Respawn Point Color"));
                EditorGUILayout.Slider(spawnRadius, 0.05f, 0.5f, new GUIContent("Respawn Point Radius"));
                EditorGUILayout.Slider(heightBox, 0.25f, 2, new GUIContent("Container Height"));
                EditorGUILayout.Slider(arrowLength, 0.25f, 1.5f, new GUIContent("Arrow Length"));
                EditorGUI.indentLevel--;
            }

            ResetLabelWidth();
        }
    }
}