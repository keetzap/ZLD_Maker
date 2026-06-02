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
        private SerializedProperty targetPositionMode;
        private SerializedProperty targetTransform;
        private SerializedProperty targetPositionVector;
        private SerializedProperty lookAtTarget;
        private SerializedProperty offsetLookAtTarget;
        private SerializedProperty draggableRenderer;
        private SerializedProperty allowBackwards;
        private SerializedProperty lockAtTheEnd;
        private SerializedProperty useSnap;
        private SerializedProperty snapValue;

        void OnEnable()
        {
            draggableByPath = (DraggableByPath)target;

            pushTimeThreshold = serializedObject.FindProperty(DraggableByPath.Fields.PushTimeThreshold);
            targetPositionMode = serializedObject.FindProperty(DraggableByPath.Fields.TargetPositionMode);
            targetTransform = serializedObject.FindProperty(DraggableByPath.Fields.TargetTransform);
            targetPositionVector = serializedObject.FindProperty(DraggableByPath.Fields.TargetPositionVector);
            lookAtTarget = serializedObject.FindProperty(DraggableByPath.Fields.LookAtTarget);
            offsetLookAtTarget = serializedObject.FindProperty(DraggableByPath.Fields.OffsetLookAtTarget);
            draggableRenderer = serializedObject.FindProperty(DraggableByPath.Fields.DraggableRenderer);
            allowBackwards = serializedObject.FindProperty(DraggableByPath.Fields.AllowBackwards);
            lockAtTheEnd = serializedObject.FindProperty(DraggableByPath.Fields.LockAtTheEnd);
            useSnap = serializedObject.FindProperty(DraggableByPath.Fields.UseSnap);
            snapValue = serializedObject.FindProperty(DraggableByPath.Fields.SnapValue);
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
            EditorGUILayout.PropertyField(targetPositionMode);

            var mode = (DraggableByPath.TargetPositionMode)targetPositionMode.enumValueIndex;

            if (mode == DraggableByPath.TargetPositionMode.Transform)
            {
                EditorGUILayout.PropertyField(targetTransform);

                bool hasTarget = targetTransform.objectReferenceValue != null;
                if (!hasTarget)
                {
                    EditorGUILayout.HelpBox("A Target Transform must be assigned!", MessageType.Warning, true);
                    return;
                }
            }
            else
            {
                EditorGUILayout.PropertyField(targetPositionVector, new GUIContent("Target Offset (Local)"));
                EditorGUILayout.PropertyField(useSnap, new GUIContent("Enable Snapping"));
                if (useSnap.boolValue)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(snapValue, new GUIContent("Snap Amount"));
                    EditorGUI.indentLevel--;
                }
            }

            EditorGUILayout.Space(2);

            EditorGUILayout.PropertyField(allowBackwards, new GUIContent("Allow reverse direction"));
            EditorGUILayout.PropertyField(lockAtTheEnd, new GUIContent("Lock at destination"));

            if (mode == DraggableByPath.TargetPositionMode.LocalOffset || (mode == DraggableByPath.TargetPositionMode.Transform && draggableByPath.GetComponentInParent()))
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(lookAtTarget, new GUIContent("Look at target"));
                if (EditorGUI.EndChangeCheck())
                {
                    if (!lookAtTarget.boolValue && draggableRenderer.objectReferenceValue != null)
                    {
                        GameObject rendererObj = (GameObject)draggableRenderer.objectReferenceValue;
                        Undo.RecordObject(rendererObj.transform, "Reset Rotation");
                        rendererObj.transform.localRotation = Quaternion.identity;
                    }
                }
                EditorGUI.BeginDisabledGroup(lookAtTarget.boolValue == false);
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(draggableRenderer);
                    if (lookAtTarget.boolValue && draggableRenderer.objectReferenceValue == null)
                    {
                        EditorGUILayout.HelpBox("A model must be assigned.", MessageType.Warning, true);
                    }
                    EditorGUILayout.IntSlider(offsetLookAtTarget, 0, 360, new GUIContent("Offset in Degrees"));
                    EditorGUI.indentLevel--;
                }
                EditorGUI.EndDisabledGroup();
            }
            else if (mode == DraggableByPath.TargetPositionMode.Transform && !draggableByPath.GetComponentInParent())
            {
                EditorGUILayout.HelpBox(
                    "To prevent an infinite cycle, 'Look at' parameters are not accessible.\n" +
                    "Try unparenting the Target Position from the gameObject.",
                    MessageType.Warning, true);
            }
        }

        private void OnSceneGUI()
        {
            if (Application.isPlaying) return;
            if (draggableByPath.TargetMode != DraggableByPath.TargetPositionMode.LocalOffset) return;

            Transform objTransform = draggableByPath.transform;
            Vector3 worldPos = objTransform.TransformPoint(draggableByPath.TargetPositionOffset);

            EditorGUI.BeginChangeCheck();
            Vector3 newWorldPos = Handles.PositionHandle(worldPos, objTransform.rotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(draggableByPath, "Move Target Position");
                
                Vector3 newLocalPos = objTransform.InverseTransformPoint(newWorldPos);
                
                if (draggableByPath.UseSnap && draggableByPath.SnapValue > 0f)
                {
                    newLocalPos.x = Mathf.Round(newLocalPos.x / draggableByPath.SnapValue) * draggableByPath.SnapValue;
                    newLocalPos.y = Mathf.Round(newLocalPos.y / draggableByPath.SnapValue) * draggableByPath.SnapValue;
                    newLocalPos.z = Mathf.Round(newLocalPos.z / draggableByPath.SnapValue) * draggableByPath.SnapValue;
                }
                
                draggableByPath.TargetPositionOffset = newLocalPos;
            }
        }
    }
}