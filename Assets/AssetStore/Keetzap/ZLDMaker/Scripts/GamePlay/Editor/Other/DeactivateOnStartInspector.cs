using UnityEngine;
using UnityEditor;
using Keetzap.Core;

namespace Keetzap.ZeldaMaker
{
    [CustomEditor(typeof(DeactivateOnAwake))]
    public class DeactivateOnStartInspector : BaseEditor
    {
        private DeactivateOnAwake deactivateOnAwake;

        void OnEnable()
        {
            deactivateOnAwake = (DeactivateOnAwake)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUI.BeginChangeCheck();

            Section("DEACTIVATE ON AWAKE PROPERTIES ***", SectionProperties);

            if (EditorGUI.EndChangeCheck())
                Undo.RegisterCompleteObjectUndo(deactivateOnAwake, "Deactivate on Awake");

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed) 
                EditorUtility.SetDirty(deactivateOnAwake);
        }

        private void SectionProperties()
        {
            EditorGUILayout.HelpBox(NO_PROPERTIES, MessageType.Info, true);
        }
    }
}