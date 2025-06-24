using UnityEngine;
using UnityEditor;
using Keetzap.Core;

namespace Keetzap.ZeldaMaker
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Collectable))]
    public class CollectableInspector : BaseEditor
    {
        private Collectable collectable;

        private SerializedProperty configurationFile;
        private SerializedProperty awakeFeedback;
        private SerializedProperty collectFeedback;
        private SerializedProperty autodestroyObject;
        private SerializedProperty typeOfDestruction;
        private SerializedProperty delay;

        void OnEnable()
        {
            collectable = (Collectable)target;

            configurationFile = serializedObject.FindProperty(Collectable.Fields.ConfigurationFile);
            awakeFeedback = serializedObject.FindProperty(Collectable.Fields.AwakeFeedback);
            collectFeedback = serializedObject.FindProperty(Collectable.Fields.CollectFeedback);
            autodestroyObject = serializedObject.FindProperty(Collectable.Fields.AutodestroyObject);
            typeOfDestruction = serializedObject.FindProperty(Collectable.Fields.Destruction);
            delay = serializedObject.FindProperty(Collectable.Fields.Delay);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUI.BeginChangeCheck();

            Section("COLLECTABLE PROPERTIES", SectionProperties);

            if (EditorGUI.EndChangeCheck())
                Undo.RegisterCompleteObjectUndo(collectable, "Collectable");

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed) 
                EditorUtility.SetDirty(collectable);
        }

        private void SectionProperties()
        {
            EditorGUILayout.PropertyField(configurationFile);
            EditorGUI.BeginDisabledGroup(true);
            {
                string amount = collectable.GameDataAsset() == null ? "-" : collectable.GameDataAsset().amount.ToString();
                EditorGUILayout.LabelField(string.Format($"Collect amount: {amount} ***"));
            }
            EditorGUI.EndDisabledGroup();
            Decorators.Separator();
            EditorGUILayout.Space(3);
            EditorGUILayout.PropertyField(awakeFeedback);
            EditorGUILayout.PropertyField(collectFeedback);
            EditorGUILayout.Space(3);

            EditorGUIUtility.labelWidth = 200;

            Decorators.SeparatorSimple();
            EditorGUILayout.Space(2);
            EditorGUILayout.PropertyField(autodestroyObject);

            EditorGUI.BeginDisabledGroup(!autodestroyObject.boolValue);
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(typeOfDestruction, new GUIContent("Delay on destruction"));

                float delayFromFeedback = collectable.GetDelayFromFeedback();

                if (typeOfDestruction.enumValueIndex == (int)TypeOfDestruction.AfterFeedbackDuration)
                {
                    EditorGUI.BeginDisabledGroup(true);
                    {
                        EditorGUILayout.FloatField(new GUIContent("Feedback Effect Duration"), delayFromFeedback);
                    }
                    EditorGUI.EndDisabledGroup();
                }
                else if (typeOfDestruction.enumValueIndex == (int)TypeOfDestruction.AfterDelay)
                {
                    EditorGUILayout.PropertyField(delay);
                    if (delay.floatValue < delayFromFeedback)
                    {
                        EditorGUILayout.Space(2);
                        EditorGUILayout.HelpBox(string.Format($"The value of 'Delay' is less than the duration of the Feedback Effect: {delayFromFeedback}"), MessageType.Warning, true);
                    }
                }
                EditorGUI.indentLevel--;
            }

            ResetLabelWidth();
        }
    }
}