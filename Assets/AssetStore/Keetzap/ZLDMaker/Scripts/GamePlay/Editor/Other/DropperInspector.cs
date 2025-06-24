using Keetzap.Core;
using UnityEditor;
using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SimpleDropper))]
    public class DropperInspector : BaseEditor
    {
        private SimpleDropper dropper;

        private SerializedProperty drop;
        private SerializedProperty itAlwaysDrops;
        private SerializedProperty chanceOfDropping;
        private SerializedProperty delay;
        private SerializedProperty effect;

        void OnEnable()
        {
            dropper = (SimpleDropper)target;

            drop = serializedObject.FindProperty(SimpleDropper.Fields.Drop);
            itAlwaysDrops = serializedObject.FindProperty(SimpleDropper.Fields.ItAlwaysDrops);
            chanceOfDropping = serializedObject.FindProperty(SimpleDropper.Fields.ChanceOfDropping);
            delay = serializedObject.FindProperty(SimpleDropper.Fields.Delay);
            effect = serializedObject.FindProperty(SimpleDropper.Fields.SpawnEffect);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            Section("DROPPER PROPERTIES", SectionProperties);

            if (EditorGUI.EndChangeCheck())
                Undo.RegisterCompleteObjectUndo(dropper, "Dropper");

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
                EditorUtility.SetDirty(dropper);
        }

        private void SectionProperties()
        {
            EditorGUILayout.PropertyField(drop);
            EditorGUILayout.PropertyField(itAlwaysDrops);
            EditorGUI.BeginDisabledGroup(itAlwaysDrops.boolValue == true);
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.IntSlider(chanceOfDropping, 0, 100, new GUIContent("Chance (%) of dropping ***"));
                EditorGUI.indentLevel--;
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.PropertyField(delay);
            Decorators.SeparatorSimple();
            EditorGUILayout.PropertyField(effect);
        }
    }
}