using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Keetzap.EditorTools
{
    public static class Decorators
    {
        static GUIContent tempGuiContent = new GUIContent();

        public static GUIContent TempContent(string label, Texture2D icon)
        {
            tempGuiContent.text = label;
            tempGuiContent.image = icon;
            tempGuiContent.tooltip = null;
            return tempGuiContent;
        }

        public static GUIContent TempContent(string label, string tooltip = null, Texture2D icon = null)
        {
            tempGuiContent.text = label;
            tempGuiContent.image = icon;
            tempGuiContent.tooltip = tooltip;
            return tempGuiContent;
        }

        private static GUIStyle _HeaderLabel;
        private static GUIStyle HeaderLabel
        {
            get
            {
                if (_HeaderLabel == null)
                {
                    _HeaderLabel = new GUIStyle(EditorStyles.label);
                    _HeaderLabel.fontStyle = FontStyle.Bold;

                    var gray1 = EditorGUIUtility.isProSkin ? 0.7f : 0.35f;
                    _HeaderLabel.normal.textColor = new Color(gray1, gray1, gray1);
                }
                return _HeaderLabel;
            }
        }

        private static GUIStyle _SubHeaderLabel;
        public static GUIStyle SubHeaderLabel
        {
            get
            {
                if (_SubHeaderLabel == null)
                {
                    _SubHeaderLabel = new GUIStyle(EditorStyles.label);
                    _SubHeaderLabel.fontStyle = FontStyle.Normal;
                    _SubHeaderLabel.normal.textColor = EditorGUIUtility.isProSkin ? new Color(0.5f, 0.5f, 0.5f) : new Color(0.35f, 0.35f, 0.35f);
                }
                return _SubHeaderLabel;
            }
        }

        private static GUIStyle _BigHeaderLabel;
        public static GUIStyle BigHeaderLabel
        {
            get
            {
                if (_BigHeaderLabel == null)
                {
                    _BigHeaderLabel = new GUIStyle(EditorStyles.largeLabel);
                    _BigHeaderLabel.fontStyle = FontStyle.Bold;
                    _BigHeaderLabel.fixedHeight = 30;
                }
                return _BigHeaderLabel;
            }
        }

        public static GUIStyle _LineStyle;
        public static GUIStyle LineStyle
        {
            get
            {
                if (_LineStyle == null)
                {
                    _LineStyle = new GUIStyle();
                    _LineStyle.normal.background = EditorGUIUtility.whiteTexture;
                    _LineStyle.stretchWidth = true;
                }

                return _LineStyle;
            }
        }

        public static void SeparatorSimple(float spaceBefore = 2, float spaceAfter = 3)
        {
            GUILayout.Space(spaceBefore);
            var color = EditorGUIUtility.isProSkin ? new Color(0.15f, 0.15f, 0.15f) : new Color(0.65f, 0.65f, 0.65f);
            GUILine(color, 1);
            GUILayout.Space(spaceAfter);
        }

        public static void Separator()
        {
            var colorDark = EditorGUIUtility.isProSkin ? new Color(.1f, .1f, .1f) : new Color(.3f, .3f, .3f);
            var colorBright = EditorGUIUtility.isProSkin ? new Color(.3f, .3f, .3f) : new Color(.9f, .9f, .9f);

            GUILayout.Space(4);
            GUILine(colorDark, 1);
            GUILine(colorBright, 1);
            GUILayout.Space(4);
        }

        public static void Separator(Rect position)
        {
            var colorDark = EditorGUIUtility.isProSkin ? new Color(.1f, .1f, .1f) : new Color(.3f, .3f, .3f);
            var colorBright = EditorGUIUtility.isProSkin ? new Color(.3f, .3f, .3f) : new Color(.9f, .9f, .9f);

            var lineRect = position;
            lineRect.height = 1;
            GUILine(lineRect, colorDark, 1);
            lineRect.y += 1;
            GUILine(lineRect, colorBright, 1);
        }

        public static void SeparatorBig()
        {
            GUILayout.Space(10);
            GUILine(new Color(.3f, .3f, .3f), 2);
            GUILayout.Space(1);
            GUILine(new Color(.3f, .3f, .3f), 2);
            GUILine(new Color(.85f, .85f, .85f), 1);
            GUILayout.Space(2);
        }

        public static void GUILine(float height = 2f)
        {
            GUILine(Color.black, height);
        }

        public static void GUILine(Color color, float height = 2f)
        {
            var position = GUILayoutUtility.GetRect(0f, float.MaxValue, height, height, LineStyle);

            if (Event.current.type == EventType.Repaint)
            {
                var orgColor = GUI.color;
                GUI.color = orgColor * color;
                LineStyle.Draw(position, false, false, false, false);
                GUI.color = orgColor;
            }
        }
        public static void GUILine(Rect position, Color color, float height = 2f)
        {
            if (Event.current.type == EventType.Repaint)
            {
                var orgColor = GUI.color;
                GUI.color = orgColor * color;
                LineStyle.Draw(position, false, false, false, false);
                GUI.color = orgColor;
            }
        }

        public static void Header(string header, string tooltip = null, bool expandWidth = false)
        {
            if (tooltip != null)
                EditorGUILayout.LabelField(TempContent(header, tooltip), HeaderLabel, GUILayout.ExpandWidth(expandWidth));
            else
                EditorGUILayout.LabelField(header, HeaderLabel, GUILayout.ExpandWidth(expandWidth));
        }

        public static void SubHeaderGray(string header, string tooltip = null, bool expandWidth = false)
        {
            if (tooltip != null)
                EditorGUILayout.LabelField(TempContent(header, tooltip), SubHeaderLabel, GUILayout.ExpandWidth(expandWidth));
            else
                EditorGUILayout.LabelField(header, SubHeaderLabel, GUILayout.ExpandWidth(expandWidth));
        }

        public static void HeaderBig(string header, string tooltip = null)
        {
            if (tooltip != null)
                EditorGUILayout.LabelField(TempContent(header, tooltip), BigHeaderLabel);
            else
                EditorGUILayout.LabelField(header, BigHeaderLabel);
        }
    }
}
