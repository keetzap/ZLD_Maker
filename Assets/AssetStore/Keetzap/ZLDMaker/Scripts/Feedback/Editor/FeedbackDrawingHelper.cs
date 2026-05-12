using UnityEditor;
using UnityEngine;

namespace Keetzap.Feedback
{
    public static class FeedbackDrawingHelper
    {
        private static readonly Color SplitterDark = new Color(0.12f, 0.12f, 0.12f, 1.333f);
        private static readonly Color SplitterLight = new Color(0.6f, 0.6f, 0.6f, 1.333f);
        private static Color Splitter => EditorGUIUtility.isProSkin ? SplitterDark : SplitterLight;
        
        public static void DrawSplitter()
        {
            // Helper to draw a separator line

            var splitterRect = GUILayoutUtility.GetRect(1f, 1f);

            splitterRect.xMin = 0f;
            splitterRect.width += 4f;

            if (Event.current.type != EventType.Repaint)
            {
                return;
            }

            EditorGUI.DrawRect(splitterRect, Splitter);
        }
    }
}