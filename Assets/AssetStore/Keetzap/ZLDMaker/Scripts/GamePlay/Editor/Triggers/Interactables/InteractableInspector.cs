using UnityEngine;
using UnityEditor;
using Keetzap.Core;

namespace Keetzap.ZeldaMaker
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Interactable))]
    public class InteractableInspector : TriggerInspector
    {
        private SerializedProperty showHintMessage;
        private SerializedProperty hintMessage;
        private SerializedProperty interactFeedback;
        private SerializedProperty pauseCharacter;
        private SerializedProperty hideWeapons;
        private SerializedProperty destroyAfterInteraction;

        protected override void OnEnable()
        {
            base.OnEnable();

            showHintMessage = serializedObject.FindProperty(Interactable.Fields.ShowHintMessage);
            hintMessage = serializedObject.FindProperty(Interactable.Fields.HintMessage);
            interactFeedback = serializedObject.FindProperty(Interactable.Fields.InteractFeedback);
            pauseCharacter = serializedObject.FindProperty(Interactable.Fields.PauseCharacter);
            hideWeapons = serializedObject.FindProperty(Interactable.Fields.HideWeapons);
            destroyAfterInteraction = serializedObject.FindProperty(Interactable.Fields.DestroyAfterInteraction);
        }

        protected void SectionCommonProperties()
        {
            Decorators.HeaderBig("Basic Options");
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(showHintMessage, new GUIContent("Show Hint Message ***"));
                EditorGUI.BeginDisabledGroup(showHintMessage.boolValue == false);
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(hintMessage);
                    EditorGUI.indentLevel--;
                }
                EditorGUI.EndDisabledGroup();
                EditorGUI.indentLevel--;
            }

            GUILayout.Space(2);
            Decorators.SeparatorSimple();
            Decorators.HeaderBig("Advanced Options");
            {  
                EditorGUI.indentLevel++;
                EditorGUIUtility.labelWidth = 230;
                EditorGUILayout.PropertyField(pauseCharacter, new GUIContent("Pause character interactions"));
                EditorGUILayout.PropertyField(hideWeapons, new GUIContent("Hide weapons while interacting"));
                EditorGUILayout.PropertyField(destroyAfterInteraction, new GUIContent("Single interaction (only once) ***"));
                ResetLabelWidth();
                EditorGUI.indentLevel--;
            }
        }

        protected void SectionInteractableFeedback()
        {
            EditorGUILayout.PropertyField(interactFeedback, new GUIContent("Interaction Feedback"));
        }
    }
}