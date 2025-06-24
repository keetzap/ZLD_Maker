using UnityEditor;
using UnityEngine;

namespace Keetzap.Feedback
{
    public static class FeedbackControlsEditor
    {
        private static readonly GUILayoutOption Height = GUILayout.Height(20);
        private static readonly GUILayoutOption Width = GUILayout.Width(50);
        private static readonly GUILayoutOption ExpandWidth = GUILayout.ExpandWidth(true);
        public static void Draw(bool isComponentExpanded, IFeedbackControl feedbackControl)
        {
            if(!isComponentExpanded)
            {
                return;
            }
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button(new GUIContent("Play"), Height, Width, ExpandWidth))
            {
                feedbackControl.Play();
            }
            if(GUILayout.Button(new GUIContent("Stop"), Height, Width, ExpandWidth))
            {
                feedbackControl.Stop();
            }

            DrawSaveButton(feedbackControl);
            
            EditorGUILayout.EndHorizontal();
        }

        private static void DrawSaveButton(IFeedbackControl feedbackControl)
        {
            GUI.enabled = Application.isPlaying;
            var saveTooltip = Application.isPlaying ? "Use this button to save current changes after finishing play mode" : "Save button is enabled only in runtime"; 
            if (GUILayout.Button(new GUIContent("Save", saveTooltip), Height, Width, ExpandWidth))
            {
                feedbackControl.Save();
            }
            GUI.enabled = true;
        } 
    }
}