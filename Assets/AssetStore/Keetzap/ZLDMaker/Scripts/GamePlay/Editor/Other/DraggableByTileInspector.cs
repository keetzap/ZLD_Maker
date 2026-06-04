using UnityEngine;
using UnityEditor;
using Keetzap.Core;

namespace Keetzap.ZeldaMaker
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(DraggableByTile))]
    public class DraggableByTileInspector : BaseEditor
    {
        private DraggableByTile draggableByTile;

        private SerializedProperty moveOnSingleTile;
        private SerializedProperty typeOfDirection;
        private SerializedProperty pushTimeThreshold;
        private SerializedProperty timeToReachNextTile;
        private SerializedProperty animationCurve;
        private SerializedProperty pushFeedback;
        private SerializedProperty stopFeedback;

        void OnEnable()
        {
            draggableByTile = (DraggableByTile)target;

            moveOnSingleTile = serializedObject.FindProperty(DraggableByTile.Fields.MoveOnSingleTile);
            typeOfDirection = serializedObject.FindProperty(DraggableByTile.Fields.TypeOfDirection);
            pushTimeThreshold = serializedObject.FindProperty(DraggableByTile.Fields.PushTimeThreshold);
            timeToReachNextTile = serializedObject.FindProperty(DraggableByTile.Fields.TimeToReachNextTile);
            animationCurve = serializedObject.FindProperty(DraggableByTile.Fields.AnimationCurve);
            pushFeedback = serializedObject.FindProperty(DraggableByTile.Fields.PushFeedback);
            stopFeedback = serializedObject.FindProperty(DraggableByTile.Fields.StopFeedback);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            EditorGUIUtility.labelWidth = 200;
            Section("DRAG BY TILE PROPERTIES", SectionMainProperties);
            Section("FEEDBACKS", SectionFeedbacks);

            if (EditorGUI.EndChangeCheck())
                Undo.RegisterCompleteObjectUndo(draggableByTile, "Drag by type properties");

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
                EditorUtility.SetDirty(draggableByTile);
        }

        private void SectionMainProperties()
        {
            EditorGUILayout.PropertyField(moveOnSingleTile, new GUIContent("Move One Tile Only"));
            EditorGUILayout.PropertyField(typeOfDirection, new GUIContent("Allowed Directions"));
            EditorGUILayout.Space(2);
            EditorGUILayout.PropertyField(pushTimeThreshold, new GUIContent("Push Time Threshold"));
            EditorGUILayout.PropertyField(timeToReachNextTile, new GUIContent("Time To Reach Next Tile"));
            EditorGUILayout.PropertyField(animationCurve, new GUIContent("Movement Anim. Curve"));
        }

        private void SectionFeedbacks()
        {
            EditorGUILayout.PropertyField(pushFeedback, new GUIContent("Moving Feedback"));
            EditorGUILayout.PropertyField(stopFeedback, new GUIContent("Stop Feedback"));
        }
    }
}