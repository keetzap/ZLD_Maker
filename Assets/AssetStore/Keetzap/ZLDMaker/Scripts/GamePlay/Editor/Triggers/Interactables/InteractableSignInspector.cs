using UnityEngine;
using UnityEditor;

namespace Keetzap.ZeldaMaker
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(InteractableSign))]
    public class InteractableSignInspector: InteractableInspector
    {
        private InteractableSign sign;

        private SerializedProperty typeOfMessage;
        private SerializedProperty message;

        protected override void OnEnable()
        {
            base.OnEnable();

            sign = (InteractableSign)target;

            typeOfMessage = serializedObject.FindProperty(InteractableSign.Fields.TypeOfMessage);
            message = serializedObject.FindProperty(InteractableSign.Fields.Message);
        }

        public override void OnInspectorGUI()
        {
            InitInspector();

            Section("COMMON PROPERTIES", SectionCommonProperties);
            Section("INTERACTABLE SIGN PROPERTIES", SectionSignProperties);
            Section("INTERACTABLE SIGN FEEDBACK", SectionInteractableFeedback);
            Section("DEBUG", SectionDebug);

            EndInspector(sign, "Interactable sign asset");
        }

        private void SectionSignProperties()
        {
            EditorGUILayout.PropertyField(typeOfMessage, new GUIContent("Sign type ***"));
            EditorGUILayout.LabelField("Message text ***");
            GUILayout.Space(2);
            message.stringValue = EditorGUILayout.TextArea(message.stringValue, GUILayout.MinHeight(40));
        }     
    }
}