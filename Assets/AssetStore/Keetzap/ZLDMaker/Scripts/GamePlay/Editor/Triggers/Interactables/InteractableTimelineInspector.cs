using UnityEngine;
using UnityEditor;
using Keetzap.Core;

namespace Keetzap.ZeldaMaker
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(InteractableTimeline))]
    public class InteractableTimelineInspector : InteractableInspector
    {
        private SerializedProperty initialTimeline;
        private SerializedProperty defaultInitialPosition;
        private SerializedProperty timeToRepositioning;
        private SerializedProperty openingTimeline;
        private SerializedProperty closingTimeline;
        private SerializedProperty dialogMessage;
        private SerializedProperty exitFeedback;
        private SerializedProperty endInteraction;

        protected override void OnEnable()
        {
            base.OnEnable();

            initialTimeline = serializedObject.FindProperty(InteractableTimeline.Fields.InitialTimeline);
            defaultInitialPosition = serializedObject.FindProperty(InteractableTimeline.Fields.DefaultInitialPosition);
            timeToRepositioning = serializedObject.FindProperty(InteractableTimeline.Fields.TimeToRepositioning);
            openingTimeline = serializedObject.FindProperty(InteractableTimeline.Fields.OpeningTimeline);
            closingTimeline = serializedObject.FindProperty(InteractableTimeline.Fields.ClosingTimeline);
            exitFeedback = serializedObject.FindProperty(InteractableTimeline.Fields.ExitFeedback);
            endInteraction = serializedObject.FindProperty(InteractableTimeline.Fields.EndInteraction);
            dialogMessage = serializedObject.FindProperty(InteractableTimeline.Fields.DialogMessage);
        }

        protected void SectionTimelineProperties()
        {
            EditorGUILayout.PropertyField(defaultInitialPosition);
            EditorGUILayout.PropertyField(timeToRepositioning);
            GUILayout.Space(2);
            Decorators.SeparatorSimple();
            GUILayout.Space(2);
            EditorGUILayout.PropertyField(initialTimeline);
            EditorGUILayout.PropertyField(openingTimeline);
            EditorGUILayout.PropertyField(closingTimeline);
            GUILayout.Space(2);
            Decorators.SeparatorSimple();
            GUILayout.Space(2);
            EditorGUILayout.LabelField("Dialog text:");
            GUILayout.Space(2);
            dialogMessage.stringValue = EditorGUILayout.TextArea(dialogMessage.stringValue, GUILayout.MinHeight(40));
        }

        protected void SectionInteractableTimelineFeedback()
        {
            SectionInteractableFeedback();
            EditorGUILayout.PropertyField(exitFeedback, new GUIContent("On Exit Feedback"));
            GUILayout.Space(4);
            EditorGUILayout.PropertyField(endInteraction, new GUIContent("On End Interaction"));
        }
    }
}