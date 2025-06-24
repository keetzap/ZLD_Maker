using UnityEngine;
using UnityEditor;
using Keetzap.Core;

namespace Keetzap.ZeldaMaker
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(InteractableTimeline))]
    public class InteractableTimelineInspector : InteractableInspector
    {
        private SerializedProperty defaultInitialPosition;
        private SerializedProperty timeToRepositioning;
        private SerializedProperty openingTimeline;
        private SerializedProperty closingTimeline;
        private SerializedProperty dialogMessage;
        private SerializedProperty closingFeedback;

        protected override void OnEnable()
        {
            base.OnEnable();

            defaultInitialPosition = serializedObject.FindProperty(InteractableTimeline.Fields.DefaultInitialPosition);
            timeToRepositioning = serializedObject.FindProperty(InteractableTimeline.Fields.TimeToRepositioning);
            openingTimeline = serializedObject.FindProperty(InteractableTimeline.Fields.OpeningTimeline);
            closingTimeline = serializedObject.FindProperty(InteractableTimeline.Fields.ClosingTimeline);
            closingFeedback = serializedObject.FindProperty(InteractableTimeline.Fields.ClosingFeedback);
            dialogMessage = serializedObject.FindProperty(InteractableTimeline.Fields.DialogMessage);
        }

        protected void SectionTimelineProperties()
        {
            EditorGUILayout.PropertyField(defaultInitialPosition);
            EditorGUILayout.PropertyField(timeToRepositioning);
            GUILayout.Space(2);
            Decorators.SeparatorSimple();
            GUILayout.Space(2);
            EditorGUILayout.PropertyField(openingTimeline);
            EditorGUILayout.PropertyField(closingTimeline);
            //EditorGUILayout.PropertyField(closingFeedback);
            GUILayout.Space(2);
            Decorators.SeparatorSimple();
            GUILayout.Space(2);
            EditorGUILayout.LabelField("Dialog text: ***");
            //EditorGUI.indentLevel++;
            GUILayout.Space(2);
            dialogMessage.stringValue = EditorGUILayout.TextArea(dialogMessage.stringValue, GUILayout.MinHeight(40));
            //EditorGUI.indentLevel--;
        }

        protected void SectionInteractableTimelineFeedback()
        {
            EditorGUILayout.PropertyField(closingFeedback, new GUIContent("On Closing ***"));
        }
    }
}