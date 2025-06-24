using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Presets;
using UnityEngine;

namespace Keetzap.Feedback
{
    public static class FeedbackSaveState
    {
        private static readonly Dictionary<SerializedObject, Preset> PersistantChanges = new();

        private static bool _initialized = false;

        public static void Initialize()
        {
            if (_initialized)
            {
                return;
            }
            
            _initialized = true;
            EditorApplication.playModeStateChanged += ApplyChanges;
        }

        public static void SaveChangesToPersistant(Object currentObject)
        {    
            if (!CheckInitialization())
            {
                return;
            }
            
            var savePreset = new Preset(currentObject);
            var serializedObject = new SerializedObject(currentObject);
            if (PersistantChanges.ContainsKey(serializedObject))
            {
                Debug.Log("An existing feedback effect is saved. It's gonna be overwritten.");
                PersistantChanges[serializedObject] = savePreset;
                return;
            }
            PersistantChanges.Add(serializedObject, savePreset);
        }
        
        private static void ApplyChanges(PlayModeStateChange state)
        {
            if (PersistantChanges.Count == 0)
            {
                return;
            }
            
            if (state == PlayModeStateChange.EnteredEditMode)
            {
                ApplyChanges();   
            }
        }
        
        private static void ApplyChanges()
        {
            if (!CheckInitialization())
            {
                return;
            }
            
            foreach (var persistantChange in PersistantChanges)
            {
                persistantChange.Value.ApplyTo(persistantChange.Key.targetObject);
            }
            
            PersistantChanges.Clear();
        }

        public static bool CheckInitialization()
        {
            if(!_initialized)
                Debug.LogWarning($"In order to use saving functions, {nameof(FeedbackSaveState)} class should be initialized using {nameof(Initialize)} function");
            return _initialized;
        } 
    }
}