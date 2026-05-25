//-----------------------------------------------------------------------
// SelectionHistoryPlusHelpPopup.cs
//
// Copyright 2026 Social Point SL. All rights reserved.
//
//-----------------------------------------------------------------------
using UnityEditor;
using UnityEngine;

namespace Keetzap.SelectionHistoryPlus.Editor
{
    internal sealed class SelectionHistoryPlusHelpPopup : PopupWindowContent
    {
        private const string MailAddress = "technical-art@socialpoint.es";

        public override Vector2 GetWindowSize() => new(240f, 120f);

        public override void OnGUI(Rect rect)
        {
            var titleStyle = new GUIStyle(EditorStyles.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                fontSize = 14
            };

            var textStyle = new GUIStyle(EditorStyles.label)
            {
                alignment = TextAnchor.MiddleLeft,
                wordWrap = true
            };
            var mailStyle = new GUIStyle(EditorStyles.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Italic,
                normal = { textColor = new Color(0.2f, 0.6f, 1f) }
            };

            GUILayout.Space(10);

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(10);
                EditorGUILayout.LabelField("Support & Feedback", titleStyle);
                GUILayout.Space(10);
            }

            GUILayout.Space(5);

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(10);
                EditorGUILayout.LabelField("If you have any questions, feedback or bugs, please contact us at:", textStyle);
                GUILayout.Space(10);
            }

            GUILayout.Space(10);

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(10);
                EditorGUILayout.LabelField(MailAddress, mailStyle);

                Rect mailRect = GUILayoutUtility.GetLastRect();
                EditorGUIUtility.AddCursorRect(mailRect, MouseCursor.Link);

                if (Event.current.type == EventType.MouseDown && mailRect.Contains(Event.current.mousePosition))
                {
                    Application.OpenURL($"mailto:{MailAddress}?subject={SelectionHistoryPlusLabels.SelectionHistoryPlus} Support");
                    Event.current.Use();
                }

                GUILayout.Space(10);
            }
        }
    }
}
