using UnityEditor;
using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(TriggerByProximity))]
    public class TriggerByProximityInspector : TriggerInspector
    {
        private SerializedProperty typeOfTriggerBehaviour;
        private SerializedProperty oneSingleUse;
        private SerializedProperty canBeEnabledByEnemies;

        protected override void OnEnable()
        {
            base.OnEnable();

            typeOfTriggerBehaviour = serializedObject.FindProperty(TriggerByProximity.Fields.TypeOfTriggerBehaviour);
            oneSingleUse = serializedObject.FindProperty(TriggerByProximity.Fields.OneSingleUse);
            canBeEnabledByEnemies = serializedObject.FindProperty(TriggerByProximity.Fields.CanBeEnabledByEnemies);
        }

        public override void OnInspectorGUI()
        {
            InitInspector();

            Section("TRIGGER BY PROXIMITY PROPERTIES ***", SectionExtraProperties);
            Section("TRIGGER LISTENERS ***", SectionListeners);
            Section("DEBUG", SectionDebug);

            EndInspector((TriggerByProximity)target, "Trigger by proximity asset");
        }

        protected void SectionExtraProperties()
        {
            EditorGUIUtility.labelWidth = 200;
            EditorGUILayout.PropertyField(typeOfTriggerBehaviour, new GUIContent("Behaviour type ***"));
            EditorGUILayout.PropertyField(oneSingleUse, new GUIContent("Single use ***"));
            EditorGUILayout.PropertyField(canBeEnabledByEnemies, new GUIContent("Can be triggered by enemies ***"));
            ResetLabelWidth();
        }
    }
}