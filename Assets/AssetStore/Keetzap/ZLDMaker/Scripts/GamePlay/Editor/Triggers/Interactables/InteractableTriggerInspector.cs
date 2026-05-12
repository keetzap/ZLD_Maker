using UnityEngine;
using UnityEditor;

namespace Keetzap.ZeldaMaker
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(InteractableTrigger))]
    public class InteractableTriggerInspector : InteractableInspector
    {
        private InteractableTrigger trigger;

        private SerializedProperty keyIsRequired;
        private SerializedProperty key;
        private SerializedProperty showAdditionalInfo;
        private SerializedProperty message;

        protected override void OnEnable()
        {
            base.OnEnable();

            trigger = (InteractableTrigger)target;

            keyIsRequired = serializedObject.FindProperty(InteractableTrigger.Fields.KeyIsRequired);
            key = serializedObject.FindProperty(InteractableTrigger.Fields.Key);
            showAdditionalInfo = serializedObject.FindProperty(InteractableTrigger.Fields.ShowAdditionalInfo);
            message = serializedObject.FindProperty(InteractableTrigger.Fields.AdditionalMessage);
        }

        public override void OnInspectorGUI()
        {
            InitInspector();

            Section("COMMON PROPERTIES", SectionCommonProperties);
            Section("TRIGGER PROPERTIES", SectionTriggerProperties);
            Section("TRIGGER FEEDBACK", SectionInteractableFeedback);
            Section("TRIGGER LISTENERS", SectionListeners);
            Section("DEBUG", SectionDebug);

            EndInspector(trigger, "Trigger asset");
        }

        private void SectionTriggerProperties()
        {
            EditorGUILayout.PropertyField(keyIsRequired, new GUIContent("Key needed ***"));
            EditorGUI.BeginDisabledGroup(keyIsRequired.boolValue == false);
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(key, new GUIContent("Key Type"));
                EditorGUILayout.PropertyField(showAdditionalInfo, new GUIContent("Display text on interaction ***"));
                //GUILayout.Space(5);
                //EditorGUILayout.LabelField("Text: ***");
                EditorGUI.BeginDisabledGroup(showAdditionalInfo.boolValue == false);
                {
                    GUILayout.Space(2);
                    message.stringValue = EditorGUILayout.TextArea(message.stringValue, GUILayout.MinHeight(40));
                }
                EditorGUI.EndDisabledGroup();
                EditorGUI.indentLevel--;
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}