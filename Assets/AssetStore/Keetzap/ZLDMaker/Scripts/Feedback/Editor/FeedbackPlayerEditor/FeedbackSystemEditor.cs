using Keetzap.Core;
using UnityEditor;
using UnityEngine;

namespace Keetzap.Feedback
{
    [CustomEditor(typeof(FeedbackSystem))]
    public sealed class FeedbackSystemEditor : BaseEditor
    {
        private AddNewFeedbackButton _addAddNewFeedbackButton;
        private FeedbacksInspector _feedbacksInspector;

        public SerializedObject SerializedObject => serializedObject;
        public FeedbackSystem Player { get; private set; }
        public SerializedProperty SerializedFeedbacksList { get; private set; }
        private SerializedProperty unparentFeedback;
        private SerializedProperty destroyFeedbackAfterPlaying;

        private void OnEnable()
        {
            Player = (FeedbackSystem)target;
            
            unparentFeedback = SerializedObject.FindProperty(FeedbackSystem.Fields.UnparentFeedback);
            destroyFeedbackAfterPlaying = SerializedObject.FindProperty(FeedbackSystem.Fields.DestroyFeedbackAfterPlaying);
            SerializedFeedbacksList = SerializedObject.FindProperty(FeedbackSystem.Fields.FeedbackList);

            _addAddNewFeedbackButton = new AddNewFeedbackButton(this, SerializedObject);
            _feedbacksInspector = new FeedbacksInspector(this);
            
            FeedbackSaveState.Initialize();

            CheckComponentsStability();
            CheckExistingComponents();

            ObjectFactory.componentWasAdded += CheckNewAddedComponents;
        }

        private void OnDisable()
        {
            ObjectFactory.componentWasAdded -= CheckNewAddedComponents;
        }

        private void CheckComponentsStability()
        {
            SerializedObject.Update();
            
            for (int feedbackIndex = 0; feedbackIndex < SerializedFeedbacksList.arraySize; feedbackIndex++)
            {
                var prop = SerializedFeedbacksList.GetArrayElementAtIndex(feedbackIndex);
                if (prop.objectReferenceValue == null)
                {
                    SerializedFeedbacksList.DeleteArrayElementAtIndex(feedbackIndex);
                }
            }
            
            SerializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private void CheckExistingComponents()
        {
            var feedbackEffects = Player.gameObject.GetComponents<FeedbackEffect>();
            foreach (var feedbackEffect in feedbackEffects)
            {
                if (!Player.FeedbackEffects.Contains(feedbackEffect))
                {
                    Player.FeedbackEffects.Add(feedbackEffect);
                }
            }
        }

        private void CheckNewAddedComponents(Component newComponent)
        {
            if(Player.gameObject != newComponent.gameObject) return;

            if (newComponent is not FeedbackEffect newFeedbackEffect) return;
            
            SerializedObject.Update();
            
            if (Player.FeedbackEffects.Contains(newFeedbackEffect)) return;
            
            SerializedFeedbacksList.arraySize++;
            SerializedFeedbacksList.GetArrayElementAtIndex(SerializedFeedbacksList.arraySize - 1).objectReferenceValue = newFeedbackEffect;
            SerializedObject.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI()
        {
            InitInspector();

            Section("FEEDBACK MAIN PROPERTIES", DrawFeedbackCommonProperties);
            _feedbacksInspector.Draw();
            _addAddNewFeedbackButton.Draw();

            EndInspector(Player, "Feedback System");
        }

        private void DrawFeedbackCommonProperties()
        {
            SetLabelWidth(250);
            EditorGUILayout.PropertyField(unparentFeedback, new GUIContent("Unparent feedback before playing"));
            EditorGUILayout.Space(2);
            EditorGUILayout.PropertyField(destroyFeedbackAfterPlaying, new GUIContent("Destroy feedback after playing"));
            ResetLabelWidth();
        }

        public void ForceRepaint()
        {
            Repaint();
        }
        
        public override bool RequiresConstantRepaint()
        {
            return true;
        }
    }

    public static class FeedbackEffectComponentTracker
    {
        private static void CheckNewAddedComponent(Component newComponent)
        {
            if (newComponent is FeedbackEffect feedbackEffect)
            {
                if (feedbackEffect.gameObject.GetComponent<FeedbackSystem>())
                {
                    
                }
            }
        }
    }
}