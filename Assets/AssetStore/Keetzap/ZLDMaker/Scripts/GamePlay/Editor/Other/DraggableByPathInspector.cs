using UnityEngine;
using UnityEditor;
using Keetzap.Core;

namespace Keetzap.ZeldaMaker
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(DraggableByPath))]
    public class DraggableByPathInspector : BaseEditor
    {
        private DraggableByPath draggableByPath;

        private SerializedProperty pushTimeThreshold;
        private SerializedProperty targetPosition;
        private SerializedProperty lookAtTarget;
        private SerializedProperty offsetLookAtTarget;
        private SerializedProperty draggableRenderer;
        private SerializedProperty allowBackwards;
        private SerializedProperty lockAtTheEnd;

        void OnEnable()
        {
            draggableByPath = (DraggableByPath)target;

            pushTimeThreshold = serializedObject.FindProperty(DraggableByPath.Fields.PushTimeThreshold);
            targetPosition = serializedObject.FindProperty(DraggableByPath.Fields.TargetPosition);
            lookAtTarget = serializedObject.FindProperty(DraggableByPath.Fields.LookAtTarget);
            offsetLookAtTarget = serializedObject.FindProperty(DraggableByPath.Fields.OffsetLookAtTarget);
            draggableRenderer = serializedObject.FindProperty(DraggableByPath.Fields.DraggableRenderer);
            allowBackwards = serializedObject.FindProperty(DraggableByPath.Fields.AllowBackwards);
            lockAtTheEnd = serializedObject.FindProperty(DraggableByPath.Fields.LockAtTheEnd);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            EditorGUIUtility.labelWidth = 200;
            Section("DRAG BY PATH PROPERTIES", SectionMainProperties);

            if (EditorGUI.EndChangeCheck())
                Undo.RegisterCompleteObjectUndo(draggableByPath, "Drag by path properties");

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
                EditorUtility.SetDirty(draggableByPath);
        }

        private void SectionMainProperties()
        {
            EditorGUILayout.PropertyField(pushTimeThreshold);
            EditorGUILayout.PropertyField(targetPosition);
            EditorGUILayout.Space(2);
            if (targetPosition.objectReferenceValue != null)
            {
                EditorGUILayout.PropertyField(allowBackwards, new GUIContent("Allow reverse direction ***"));
                //EditorGUI.BeginDisabledGroup(allowBackwards.boolValue == false);
                //{
                //    EditorGUI.indentLevel++;
                //    EditorGUILayout.PropertyField(allowGoBeyondInitPos, new GUIContent("Allow surpass init position"));
                //    EditorGUI.indentLevel--;
                //}
                //EditorGUI.EndDisabledGroup();
                EditorGUILayout.PropertyField(lockAtTheEnd, new GUIContent("Lock at destination"));

                if (draggableByPath.GetComponentInParent())
                {
                    EditorGUILayout.PropertyField(lookAtTarget, new GUIContent("Look at target ???"));
                    EditorGUI.BeginDisabledGroup(lookAtTarget.boolValue == false);
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(draggableRenderer);
                        if (lookAtTarget.boolValue && draggableRenderer.objectReferenceValue == null)
                        {
                            EditorGUILayout.HelpBox("A model must be assigned.", MessageType.Warning, true);
                        }
                        EditorGUILayout.IntSlider(offsetLookAtTarget, 0, 360, new GUIContent("Offset in Degrees ???"));
                        EditorGUI.indentLevel--;
                    }
                    EditorGUI.EndDisabledGroup();
                }
                else
                    EditorGUILayout.HelpBox("To prevent an infinite cycle, 'Look at' parameters are not accesible.\nTry unparenting the Target Position from the gameObject.", MessageType.Warning, true);
            }
            else
                EditorGUILayout.HelpBox("A Target Position must be assigned!", MessageType.Warning, true);
        }
    }
}