using UnityEngine;
using UnityEditor;
using Keetzap.Core;

namespace Keetzap.ZeldaMaker
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Listener))]
    public class ListenerInspector : BaseEditor
    {
        private Listener listener;

        private SerializedProperty initialState;
        private SerializedProperty hasCooldown;
        private SerializedProperty cooldown;
        private SerializedProperty loopBehavior;
        private SerializedProperty typeOfEnd;
        private SerializedProperty numberOfOccurrences;
        private SerializedProperty timeOn;
        private SerializedProperty interval;
        private SerializedProperty delayOnTrigger;
        private SerializedProperty delayOnRelease;
        private SerializedProperty triggerFeedback;
        private SerializedProperty cooldownFeedback;
        private SerializedProperty releaseFeedback;

        protected virtual void OnEnable()
        {
            listener = (Listener)target;

            initialState = serializedObject.FindProperty(Listener.Fields.InitialState);
            hasCooldown = serializedObject.FindProperty(Listener.Fields.HasCooldown);
            cooldown = serializedObject.FindProperty(Listener.Fields.Cooldown);
            loopBehavior = serializedObject.FindProperty(Listener.Fields.LoopBehavior);
            typeOfEnd = serializedObject.FindProperty(Listener.Fields.TypeOfEnd);
            numberOfOccurrences = serializedObject.FindProperty(Listener.Fields.NumberOfOccurrences);
            timeOn = serializedObject.FindProperty(Listener.Fields.TimeOn);
            interval = serializedObject.FindProperty(Listener.Fields.Interval);
            delayOnTrigger = serializedObject.FindProperty(Listener.Fields.DelayOnTrigger);
            delayOnRelease = serializedObject.FindProperty(Listener.Fields.DelayOnRelease);

            triggerFeedback = serializedObject.FindProperty(Listener.Fields.TriggerFeedback);
            cooldownFeedback = serializedObject.FindProperty(Listener.Fields.CooldownFeedback);
            releaseFeedback = serializedObject.FindProperty(Listener.Fields.ReleaseFeedback);
        }

        public override void OnInspectorGUI()
        {
            InitInspector();

            Section("COMMON PROPERTIES", SectionMainProperties);
            Section("LISTENER FEEDBACK ***", SectionFeedbacksProperties);

            EndInspector(listener, "Listener asset");
        }

        protected void SectionMainProperties()
        {
            EditorGUILayout.PropertyField(initialState);
            EditorGUI.BeginDisabledGroup(true);
            if (Application.isPlaying)
            {
                EditorGUILayout.LabelField(string.Format($"Current state:\t{listener.GetCurrentState()}"));
                Repaint();
            }
            else
            {
                EditorGUILayout.LabelField("Current state:\t---");
            }
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space(3);
            Decorators.SeparatorSimple();

            EditorGUILayout.PropertyField(hasCooldown);
            EditorGUI.BeginDisabledGroup(hasCooldown.boolValue == false);
            EditorGUILayout.PropertyField(cooldown);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space(3);
            Decorators.SeparatorSimple();

            EditorGUILayout.PropertyField(loopBehavior);
            EditorGUI.BeginDisabledGroup(loopBehavior.boolValue == false);
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.Space(2);
                EditorGUIUtility.labelWidth = 270;

                if (hasCooldown.boolValue && loopBehavior.boolValue)
                {
                    EditorGUILayout.HelpBox("The end of the loop behaviour is driven by the cooldown value", MessageType.Info);
                }
                else
                {
                    ResetLabelWidth();
                    EditorGUILayout.PropertyField(typeOfEnd, new GUIContent("End condition ***"));
                    EditorGUIUtility.labelWidth = 270;

                    EditorGUI.BeginDisabledGroup(typeOfEnd.intValue == 0);
                    {
                        EditorGUILayout.PropertyField(numberOfOccurrences, new GUIContent("Number of occurrences ???"));
                    }
                    EditorGUI.EndDisabledGroup();
                }

                EditorGUILayout.PropertyField(timeOn, new GUIContent("Enabled time"));
                EditorGUILayout.PropertyField(interval, new GUIContent("Interval time between occurrences ???"));
                ResetLabelWidth();
                EditorGUI.indentLevel--;
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space(3);
            Decorators.SeparatorSimple();

            EditorGUILayout.PropertyField(delayOnTrigger, new GUIContent("Delay after Trigger ***"));
            EditorGUILayout.PropertyField(delayOnRelease, new GUIContent("Delay On Release ???"));
        }

        protected void SectionFeedbacksProperties()
        {
            EditorGUILayout.PropertyField(triggerFeedback);
            EditorGUILayout.PropertyField(cooldownFeedback);
            EditorGUILayout.PropertyField(releaseFeedback);
        }
    }
}