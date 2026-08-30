using UnityEditor;
using UnityEngine;
using Keetzap.Core;

namespace Keetzap.ZeldaMaker
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SpikesTrap))]
    public class SpikesTrapInspector : BaseEditor
    {
        private SpikesTrap spikesTrap;

        private SerializedProperty boxCollider;
        private SerializedProperty lifeCost;
        private SerializedProperty allowMovement;
        private SerializedProperty safePoint;
        private SerializedProperty timeToRespawn;
        private SerializedProperty jumpHeight;
        private SerializedProperty damageInterval;

        private void OnEnable()
        {
            spikesTrap = (SpikesTrap)target;

            boxCollider = serializedObject.FindProperty(SpikesTrap.Fields.BoxCollider);
            lifeCost = serializedObject.FindProperty(SpikesTrap.Fields.LifeCost);
            allowMovement = serializedObject.FindProperty(SpikesTrap.Fields.AllowMovement);
            safePoint = serializedObject.FindProperty(SpikesTrap.Fields.SafePoint);
            timeToRespawn = serializedObject.FindProperty(SpikesTrap.Fields.TimeToRespawn);
            jumpHeight = serializedObject.FindProperty(SpikesTrap.Fields.JumpHeight);
            damageInterval = serializedObject.FindProperty(SpikesTrap.Fields.DamageInterval);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            Section("SPIKES TRAP PROPERTIES", SectionProperties);

            if (EditorGUI.EndChangeCheck())
                Undo.RegisterCompleteObjectUndo(spikesTrap, "Spikes Trap");

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
                EditorUtility.SetDirty(spikesTrap);
        }

        private void SectionProperties()
        {
            if (boxCollider.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("A BoxCollider must be assigned!", MessageType.Warning, true);

                Color originalColor = GUI.contentColor;
                GUI.contentColor = new Color(1, 0.5f, 0.5f, 1);
                EditorGUILayout.PropertyField(boxCollider);
                GUI.contentColor = originalColor;
            }
            else
            {
                EditorGUILayout.PropertyField(boxCollider);
            }

            EditorGUILayout.PropertyField(lifeCost);
            EditorGUILayout.PropertyField(allowMovement, new GUIContent("Allow Movement"));

            if (allowMovement.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(damageInterval, new GUIContent("Damage Interval (s)"));
                EditorGUI.indentLevel--;
            }
            else
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(safePoint);
                EditorGUILayout.PropertyField(timeToRespawn);
                EditorGUILayout.PropertyField(jumpHeight);
                EditorGUI.indentLevel--;
            }
        }
    }
}
