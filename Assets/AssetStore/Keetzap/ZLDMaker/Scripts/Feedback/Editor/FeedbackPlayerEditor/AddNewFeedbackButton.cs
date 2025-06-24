using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Keetzap.Feedback
{
    public sealed class AddNewFeedbackButton
    {
        private const string _addNewFeedbackAction = "Add Feedback component";
        private const string _addNewFeedbackLabel = "Add new feedback...";
        
        private readonly FeedbackSystemEditor _editor;
        private readonly SerializedObject _serializedObject;

        private Dictionary<string, Type> _feedbackTypes;
        private List<string> _feedbackNames;
        
        public AddNewFeedbackButton(FeedbackSystemEditor feedbackPlayerEditor, SerializedObject serializedObject)
        {
            _editor = feedbackPlayerEditor;
            _serializedObject = serializedObject;

            PrepareFeedbackTypeList();
        }
        
        private void PrepareFeedbackTypeList()
        {
            // Retrieve available feedbacks
            List<Type> types = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                from assemblyType in domainAssembly.GetTypes()
                where assemblyType.IsSubclassOf(typeof(FeedbackEffect))
                select assemblyType).ToList();
            
            // Create display list from types
            _feedbackTypes ??= new Dictionary<string, Type>();
            _feedbackTypes.Clear();
            foreach (var feedbackType in types)
            {
                var feedbackPath = FeedbackEffectAttribute.GetFeedbackDefaultPath(feedbackType);
                _feedbackTypes.Add(feedbackPath, feedbackType);
            }

            _feedbackNames ??= new List<string>();
            _feedbackNames.Clear();
            _feedbackNames.Add(_addNewFeedbackLabel);
            foreach (var feedbackType in _feedbackTypes)
            {
                _feedbackNames.Add(feedbackType.Key);
            }
        }
        
        public void Draw()
        {
            FeedbackDrawingHelper.DrawSplitter();
            EditorGUILayout.Space();
	        
            int newItem = EditorGUILayout.Popup(0, _feedbackNames.ToArray()) - 1;
            if (newItem >= 0)
            {
                AddNewFeedback(_feedbackTypes[_feedbackNames[newItem + 1]]);
                _editor.ForceRepaint();
            }
            
            EditorGUILayout.Space();
        }

        private void AddNewFeedback(Type feedbackType)
        {
            _serializedObject.Update();
            
            Undo.RecordObject(_editor.Player.gameObject, _addNewFeedbackAction);
            var component = Undo.AddComponent(_editor.Player.gameObject, feedbackType);
            var componentSerialized = new SerializedObject(component);
            componentSerialized.Update();
            var componentExpanded = componentSerialized.FindProperty(FeedbackEffect.Fields.Expanded);
            var componentDisplayColor = componentSerialized.FindProperty(FeedbackEffect.Fields.DisplayColor);
            var componentDisplayLabel = componentSerialized.FindProperty(FeedbackEffect.Fields.DisplayLabel);
            componentDisplayColor.isExpanded = true;
            componentDisplayLabel.isExpanded = true;
            
            componentExpanded.boolValue = true;
            componentSerialized.ApplyModifiedPropertiesWithoutUndo();

            var feedbacksList = _editor.SerializedFeedbacksList;
            feedbacksList.InsertArrayElementAtIndex(feedbacksList.arraySize);
            feedbacksList.GetArrayElementAtIndex(feedbacksList.arraySize - 1).objectReferenceValue = component;
            
            _serializedObject.ApplyModifiedProperties();
            PrefabUtility.RecordPrefabInstancePropertyModifications(_editor.Player.gameObject);
        }
    }
}