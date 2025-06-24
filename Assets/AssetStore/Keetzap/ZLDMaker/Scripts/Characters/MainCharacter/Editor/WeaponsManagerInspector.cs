using Keetzap.Core;
using UnityEditor;
using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    [CustomEditor(typeof(WeaponsManager))]
    public class WeaponsManagerInspector : BaseEditor
    {
        private WeaponsManager weaponsManager;

        private SerializedProperty leftHand;
        private SerializedProperty rightHand;
        private SerializedProperty leftHandAnchor;
        private SerializedProperty rightHandAnchor;

        private bool onButtonPressed;

        void OnEnable()
        {
            weaponsManager = (WeaponsManager)target;

            leftHand = serializedObject.FindProperty("leftHand");
            rightHand = serializedObject.FindProperty("rightHand");
            leftHandAnchor = serializedObject.FindProperty("leftHandAnchor");
            rightHandAnchor = serializedObject.FindProperty("rightHandAnchor");

            onButtonPressed = false;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            Section("PROPERTIES", SectionMainProperties);

            if (EditorGUI.EndChangeCheck())
                Undo.RegisterCompleteObjectUndo(weaponsManager, "Weapons Manager Properties");

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
                EditorUtility.SetDirty(weaponsManager);
        }

        private void SectionMainProperties()
        {
            EditorGUILayout.PropertyField(leftHand);
            EditorGUILayout.PropertyField(rightHand);
            EditorGUILayout.Space(5);

            EditorGUILayout.PropertyField(leftHandAnchor);
            EditorGUILayout.PropertyField(rightHandAnchor);
            EditorGUILayout.Space(5);

            if (GUILayout.Button("Autofill properties", GUILayout.MaxHeight(24)))
            {
                AssignAnchor("L Hand", leftHandAnchor);
                AssignAnchor("R Hand", rightHandAnchor);

                onButtonPressed = true;
            }

            if (onButtonPressed)
            {
                if (leftHandAnchor.objectReferenceValue == null)
                    EditorGUILayout.HelpBox("Left Hand Anchor cannot be found!", MessageType.Warning, true);

                if (rightHandAnchor.objectReferenceValue == null)
                    EditorGUILayout.HelpBox("Right Hand Anchor cannot be found!", MessageType.Warning, true);
            }
        }

        private Transform GetHand(string pattern)
        {
            Transform model = weaponsManager.transform.parent.transform.Find("Model");

            foreach (Transform child in model.GetComponentsInChildren<Transform>())
            {
                if (child.name.Contains(pattern))
                    return child;
            }

            return null;
        }

        private void AssignAnchor(string pattern, SerializedProperty anchor)
        {
            Transform hand = GetHand(pattern);

            if (hand != null)
                anchor.objectReferenceValue = hand;
        }
    }
}