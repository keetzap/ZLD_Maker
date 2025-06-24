using UnityEngine;
using UnityEditor;
using Keetzap.Core;

namespace Keetzap.ZeldaMaker
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(RoomTransition))]
    public class RoomTransitionInspector : BaseEditor
    {
        private RoomTransition roomTransition;

        private SerializedProperty spawnPoints;
        private SerializedProperty transitionSpeed;
        private SerializedProperty showGizmos;
        private SerializedProperty detectionColor, opacity;
        private SerializedProperty spawnColor, spawnRadius;

        void OnEnable()
        {
            roomTransition = (RoomTransition)target;

            spawnPoints = serializedObject.FindProperty(RoomTransition.FieldNames.SpawnPoints);
            transitionSpeed = serializedObject.FindProperty(RoomTransition.FieldNames.TransitionSpeed);
            showGizmos = serializedObject.FindProperty(RoomTransition.FieldNames.ShowGizmos);
            detectionColor = serializedObject.FindProperty(RoomTransition.FieldNames.DetectionColor);
            opacity = serializedObject.FindProperty(RoomTransition.FieldNames.Opacity);
            spawnColor = serializedObject.FindProperty(RoomTransition.FieldNames.SpawnColor);
            spawnRadius = serializedObject.FindProperty(RoomTransition.FieldNames.SpawnRadius);
        }

        public override void OnInspectorGUI()
        {
            InitInspector();

            Section("ROOM TRANSITION PROPERTIES ***", SectionBehaviour);
            Section("DEBUG", SectionDebug);

            EndInspector(roomTransition, "Room Transition asset");
        }

        private void SectionBehaviour()
        {
            SetLabelWidth(200);

            EditorGUILayout.Slider(spawnPoints, 0, 5, new GUIContent("Spawn Points Distance"));
            spawnPoints.floatValue = Mathf.Clamp(spawnPoints.floatValue, 0, 5);
            EditorGUILayout.PropertyField(transitionSpeed);
        }

        private void SectionDebug()
        {
            EditorGUILayout.PropertyField(showGizmos);
            GUILayout.Space(2);

            if (showGizmos.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(detectionColor, new GUIContent("Collider Color"));
                EditorGUILayout.Slider(opacity, 0, 1);
                EditorGUILayout.PropertyField(spawnColor, new GUIContent("Spawn Points Color"));
                EditorGUILayout.Slider(spawnRadius, 0, 1, new GUIContent("Spawn Points Radius"));

                EditorGUI.indentLevel--;
            }

            ResetLabelWidth();
        }
    }
}