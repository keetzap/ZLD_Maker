using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Keetzap.Feedback
{
    internal class FeedbacksInspector
    {
        private readonly Color _draggedColor = new Color(0, 1, 1, 0.2f);

        private readonly FeedbackSystemEditor _editor;
        
        private int _draggedStartID = -1;
        private int _draggedEndID = -1;
        private List<float> _durations = new();

        private static readonly GUIContent FeedbackRemoveGUIContent = new GUIContent("Remove");
        private static readonly GUIContent FeedbackCloneGUIContent = new GUIContent("Clone");

        public FeedbacksInspector(FeedbackSystemEditor feedbackPlayerEditor)
        {
            _editor = feedbackPlayerEditor;
        }

        public void Draw()
        {
            var inspectorSize = EditorGUILayout.BeginVertical();
            FeedbackDrawingHelper.DrawSplitter();

            var serializedFeedbacksList = _editor.SerializedFeedbacksList;
            if (serializedFeedbacksList.arraySize == 0)
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.HelpBox("Add a new feedback to edit its settings.", MessageType.Info);
            }
            
            for (int feedbackIndex = 0; feedbackIndex < serializedFeedbacksList.arraySize; feedbackIndex++)
            {
                var feedbackProperty = serializedFeedbacksList.GetArrayElementAtIndex(feedbackIndex);
                if(feedbackProperty.objectReferenceValue == null)
                    continue;

                DrawFeedbackElement(feedbackProperty, feedbackIndex);
            }
            
            EditorGUILayout.EndVertical();
            
            HandleReordering(_editor.SerializedFeedbacksList, inspectorSize);
            
            if (serializedFeedbacksList.arraySize > 0)
            {
                var totalDuration = _editor.Player.totalDuration;
                totalDuration = _durations.OrderByDescending(x => x).First();

                FeedbackDrawingHelper.DrawSplitter();
                EditorGUILayout.Space(2);
                EditorGUILayout.LabelField(string.Format($"Total Feedback duration: {totalDuration}s."));

                _editor.Player.totalDuration = totalDuration;

                _durations.Clear();
            }

            //FeedbackControlsEditor.Draw(true, new FeedbackGeneralControlEditor(_editor.Player));
            EditorGUILayout.Space();
        }

        private void DrawFeedbackElement(SerializedProperty feedbackProperty, int feedbackIndex)
        {
            var feedbackEffectSerialized = new SerializedObject(feedbackProperty.objectReferenceValue);
            var feedbackEffect = feedbackProperty.objectReferenceValue as FeedbackEffect;
            var controlsEditor = new FeedbackElementControlsEditor(feedbackEffect);
            
            var headerRect = DrawHeader(feedbackProperty, feedbackEffectSerialized, feedbackIndex);
            DrawMainInspector(feedbackProperty, feedbackEffectSerialized);
            //FeedbackControlsEditor.Draw(IsFeedbackElementExpanded(feedbackEffectSerialized), controlsEditor);
            HandleEvents(headerRect, feedbackIndex);

            _durations.Add(feedbackEffect.GetFeedbackEffectDuration());
            
            EditorGUILayout.Space();
        }

        private void HandleEvents(Rect headerRect, int feedbackIndex)
        {
            var currentEvent = Event.current;
            switch (currentEvent.type)
            {
                case EventType.MouseDown:
                    if (headerRect.Contains(currentEvent.mousePosition))
                    {
                        _draggedStartID = feedbackIndex;
                        currentEvent.Use();
                    }
                    break;
            }

            // Draw blue rect if feedback is being dragged
            if (_draggedStartID == feedbackIndex && headerRect != Rect.zero)
            {
                EditorGUI.DrawRect(headerRect, _draggedColor);
            }
            
            //var inspectorRect = EditorGUILayout.GetControlRect();
            //Debug.Log(!inspectorRect.Contains(currentEvent.mousePosition));
            
            // If hovering at the top of the feedback while dragging one, check where the feedback should be dropped : top or bottom
            if (headerRect.Contains(currentEvent.mousePosition))
            {
                if (_draggedStartID >= 0)
                {
                    _draggedEndID = feedbackIndex;
                    Rect headerSplit = headerRect;
                    headerSplit.height *= 0.5f;
                    headerSplit.y += headerSplit.height;
                    if (headerSplit.Contains(currentEvent.mousePosition))
                    {
                        _draggedEndID = feedbackIndex + 1;
                    }
                }
            }
        }

        private Rect DrawHeader(SerializedProperty feedbackProperty, SerializedObject feedbackEffectSerialized, int feedbackIndex)
        {
            var componentEnabled = feedbackEffectSerialized.FindProperty(FeedbackEffect.Fields.Enabled);
            var componentExpanded = feedbackEffectSerialized.FindProperty(FeedbackEffect.Fields.Expanded);
            var componentDelay = feedbackEffectSerialized.FindProperty(FeedbackEffect.Fields.Delay);
            var componentDisplayColor = feedbackEffectSerialized.FindProperty(FeedbackEffect.Fields.DisplayColor);
            var componentDisplayLabel = feedbackEffectSerialized.FindProperty(FeedbackEffect.Fields.DisplayLabel);

            feedbackEffectSerialized.Update();

            FeedbackInspectorHeader.HeaderParameters parameters;
            parameters.Title = feedbackProperty.objectReferenceValue.GetType().Name;
            parameters.Enabled = componentEnabled.boolValue;
            parameters.Expanded = componentExpanded.boolValue;
            parameters.Color = FeedbackEffectAttribute.GetFeedbackColor(feedbackEffectSerialized.targetObject.GetType());
            parameters.DisplayFullHeaderColor = FeedbackEffectAttribute.GetDisplayColorFullHeader(feedbackEffectSerialized.targetObject.GetType());
            parameters.DisplayColor = componentDisplayColor.colorValue;
            parameters.DisplayLabel = componentDisplayLabel.stringValue;
            parameters.Delay = componentDelay.floatValue.ToString("F2") + "s";
            parameters.GenericMenu = menu =>
            {
                menu.AddItem(FeedbackRemoveGUIContent, false, () => RemoveFeedback(feedbackIndex));
                menu.AddItem(FeedbackCloneGUIContent, false, () => CloneFeedback(feedbackIndex));

                /*
                if (Application.isPlaying)
                    menu.AddItem(_feedbackPlayGUIContent, false, () => PlayFeedback(i));
                else
                    menu.AddDisabledItem(_feedbackPlayGUIContent);
                menu.AddSeparator(null);
                
                menu.AddItem(_feedbackResetGUIContent, false, () => ResetContextMenuFeedback(i));
                menu.AddSeparator(null);
                menu.AddItem(_feedbackDuplicateGUIContent, false, () => DuplicateFeedback(i));
                menu.AddItem(_feedbackCopyGUIContent, false, () => CopyFeedback(i));
                if (MMF_PlayerCopy.HasCopy())
                    menu.AddItem(_feedbackPasteGUIContent, false, PasteAsNew);
                else
                    menu.AddDisabledItem(_feedbackPasteGUIContent);*/
            };
            parameters.PresetObject = feedbackProperty.objectReferenceValue;

            var result = FeedbackInspectorHeader.DrawHeader(parameters);
            componentEnabled.boolValue = result.ComponentEnabled;
            feedbackEffectSerialized.ApplyModifiedProperties();
            componentExpanded.boolValue = result.ComponentExpanded;
            feedbackEffectSerialized.ApplyModifiedPropertiesWithoutUndo();
            return result.HeaderRect;
        }



        private void DrawMainInspector(SerializedProperty feedbackProperty, SerializedObject feedbackEffectSerialized)
        {
            if (!IsFeedbackElementExpanded(feedbackEffectSerialized))
            {
                return;
            }

            var editor = Editor.CreateEditor(feedbackProperty.objectReferenceValue);
            var serializedObject = editor.serializedObject;
            serializedObject.Update();
            editor.OnInspectorGUI();
            serializedObject.ApplyModifiedProperties();
        }

        private bool IsFeedbackElementExpanded(SerializedObject feedbackEffect)
        {
            var componentExpanded = feedbackEffect.FindProperty(FeedbackEffect.Fields.Expanded);
            return componentExpanded.boolValue;
        }
        
        private void HandleReordering(SerializedProperty feedbackList, Rect inspectorSize)
        {
                var currentEvent = Event.current;
                if (_draggedStartID >= 0 && _draggedEndID >= 0)
                {
                    {
                        var mousePosition = currentEvent.mousePosition;
                        float width = 100;
                        float height = 20;
                        var rect = new Rect(mousePosition.x + 10, mousePosition.y, width, height);
                        var draggingName = feedbackList.GetArrayElementAtIndex(_draggedStartID).objectReferenceValue.GetType().Name;
                        GUI.Label(rect, $"{draggingName}: {_draggedStartID}");
                    }
                    
                    if (_draggedEndID != _draggedStartID)
                    {
                        if (_draggedEndID > _draggedStartID)
                            _draggedEndID--;
                        feedbackList.MoveArrayElement(_draggedStartID, _draggedEndID);
                        _draggedStartID = _draggedEndID;    
                        
                        feedbackList.serializedObject.ApplyModifiedProperties();
                    }
                }

                bool outOfWindow = false;
                if (inspectorSize.x != 0)
                {
                    inspectorSize.width += inspectorSize.x;
                    inspectorSize.height += inspectorSize.y;
                    inspectorSize.x = 0;
                    inspectorSize.y = 0;
                    
                    if (!inspectorSize.Contains(currentEvent.mousePosition))
                    {
                        outOfWindow = true;
                    }
                }
                
                if (_draggedStartID >= 0 || _draggedEndID >= 0)
                {
                    if (outOfWindow)
                    {
                        _draggedStartID = -1;
                        _draggedEndID = -1;
                    }

                    switch (currentEvent.type)
                    {
                        case EventType.MouseUp:
                            _draggedStartID = -1;
                            _draggedEndID = -1;
                            currentEvent.Use();
                            break;
                    }
                }
        }
        
        private void RemoveFeedback(int feedbackIndex)
        {
            _editor.SerializedObject.Update();
            Undo.RecordObject(_editor.Player.gameObject, "Remove feedback component");
            var feedbackProperty = _editor.SerializedFeedbacksList.GetArrayElementAtIndex(feedbackIndex);
            Undo.DestroyObjectImmediate(feedbackProperty.objectReferenceValue);
            _editor.SerializedFeedbacksList.DeleteArrayElementAtIndex(feedbackIndex);
            _editor.SerializedObject.ApplyModifiedProperties();
            PrefabUtility.RecordPrefabInstancePropertyModifications(_editor.Player.gameObject);
        }
        
        private void CloneFeedback(int feedbackIndex)
        {
            _editor.SerializedObject.Update();
            Undo.RecordObject(_editor.Player.gameObject, "Clone feedback component");
            var cloneTarget = _editor.SerializedFeedbacksList.GetArrayElementAtIndex(feedbackIndex).objectReferenceValue;
            var cloneSerialized = new SerializedObject(cloneTarget);
            var newComponent = Undo.AddComponent(_editor.Player.gameObject, cloneTarget.GetType());
            var newComponentSerialized = new SerializedObject(newComponent);
            newComponentSerialized.Update();

            var serializedProperty = cloneSerialized.GetIterator();
            if (serializedProperty.NextVisible(true))
            {
                do
                {
                    newComponentSerialized.CopyFromSerializedProperty(serializedProperty);
                } while (serializedProperty.NextVisible(false));
            }
            newComponentSerialized.ApplyModifiedProperties();

            var expandedProperty = newComponentSerialized.FindProperty(FeedbackEffect.Fields.Expanded);
            expandedProperty.boolValue = true;
            newComponentSerialized.ApplyModifiedPropertiesWithoutUndo();
            
            _editor.SerializedFeedbacksList.InsertArrayElementAtIndex(feedbackIndex + 1);
            _editor.SerializedFeedbacksList.GetArrayElementAtIndex(feedbackIndex + 1).objectReferenceValue = newComponent;
            _editor.SerializedObject.ApplyModifiedProperties();
        }
    }
}