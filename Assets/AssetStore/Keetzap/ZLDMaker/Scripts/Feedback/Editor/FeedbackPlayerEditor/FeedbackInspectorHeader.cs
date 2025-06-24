using UnityEditor;
using UnityEditor.Presets;
using UnityEngine;
using UnityEngine.Serialization;

namespace Keetzap.Feedback
{
    internal class FeedbackInspectorHeader
    {
        private const float _headerElementsOffset = 4f;
        private const float _timingRectWidth = 150f;

        private static readonly Color HeaderBackgroundDark = new Color(0.1f, 0.1f, 0.1f, 0.2f);
        private static readonly Color HeaderBackgroundLight = new Color(1f, 1f, 1f, 0.4f);

        private static Color HeaderBackground =>  EditorGUIUtility.isProSkin ? HeaderBackgroundDark : HeaderBackgroundLight;

        private static readonly GUIStyle SmallTickBox = new GUIStyle("ShurikenToggle");

        private static readonly Color ReorderDark = new Color(1f, 1f, 1f, 0.2f);
        private static readonly Color ReorderLight = new Color(0.1f, 0.1f, 0.1f, 0.2f);
        private static Color Reorder => EditorGUIUtility.isProSkin ? ReorderDark : ReorderLight;

        private static Texture2D _paneOptionsIconDark;
        private static Texture2D _paneOptionsIconLight;

        private static Texture2D PaneOptionsIconDark
        {
            get
            {
                if (_paneOptionsIconDark == null)
                {
                    _paneOptionsIconDark =
                        (Texture2D)EditorGUIUtility.Load("Builtin Skins/DarkSkin/Images/pane options.png");
                }

                return _paneOptionsIconDark;
            }
        }

        private static Texture2D PaneOptionsIconLight
        {
            get
            {
                if (_paneOptionsIconLight == null)
                {
                    _paneOptionsIconLight =
                        (Texture2D)EditorGUIUtility.Load("Builtin Skins/LightSkin/Images/pane options.png");
                }

                return _paneOptionsIconLight;
            }
        }

        private static Texture2D PaneOptionsIcon =>
            EditorGUIUtility.isProSkin ? PaneOptionsIconDark : PaneOptionsIconLight;

        private static readonly GenericMenu GenericMenu = new();

        private static Texture2D SettingsIcon =>
            (Texture2D)EditorGUIUtility.Load("Builtin Skins/LightSkin/Images/_Popup.png");

        public struct HeaderParameters
        {
            public string Title;
            public bool Expanded;
            public bool Enabled;
            public Color Color;
            public bool DisplayFullHeaderColor;
            public Color DisplayColor;
            public System.Action<GenericMenu> GenericMenu;
            public Object PresetObject;
            public string Delay;
            public string DisplayLabel;
        }

        public struct HeaderResult
        {
            public Rect HeaderRect;
            public bool ComponentEnabled;
            public bool ComponentExpanded;
        }

        public static HeaderResult DrawHeader(HeaderParameters headerParameters)
        {
            HeaderResult result;

            var headerRect = GUILayoutUtility.GetRect(1f, 17f);
            DrawBackground(headerRect, headerParameters.DisplayFullHeaderColor, headerParameters.Color, headerParameters.DisplayColor, out var backgroundColor);
            DrawTitle(headerRect, headerParameters.Title, headerParameters.DisplayLabel, backgroundColor, out var titleRect);
            DrawColorTag(titleRect, headerParameters.Color);
            result.ComponentExpanded = DrawFoldoutIcon(headerRect, headerParameters.Expanded);
            result.ComponentEnabled = DrawEnabledStateCheckbox(headerRect, headerParameters.Enabled);
            DrawMenuButton(titleRect, out var menuButtonRect);
            DrawPresetButton(titleRect, headerParameters.PresetObject);
            DrawDelayInformation(titleRect, headerParameters.Delay, backgroundColor);
            DrawReorderButton(headerRect);

            HandleEvents(headerRect, menuButtonRect, headerParameters.GenericMenu, ref result.ComponentExpanded);

            // Background rect should be full-width
            headerRect.xMin = 0f;
            headerRect.width += 4f;
            result.HeaderRect = headerRect;
            
            return result;
        }

        private static void DrawPresetButton(Rect labelRect, Object presetTarget)
        {
            var presetButtonRect = labelRect; 
            presetButtonRect.width = PaneOptionsIcon.width;
            presetButtonRect.height = PaneOptionsIcon.height;
            presetButtonRect.x = labelRect.xMax - 16f;
            presetButtonRect.y = labelRect.y + 2;
            PresetSelector.DrawPresetButton(presetButtonRect, new [] { presetTarget });

            //GUI.DrawTexture(presetButtonRect, SettingsIcon);
        }

        private static void HandleEvents(Rect headerRect, Rect menuButtonRect, System.Action<GenericMenu> fillGenericMenu, ref bool componentExpanded)
        {
            var currentEvent = Event.current;
            if (currentEvent.type == EventType.MouseDown && menuButtonRect.Contains(currentEvent.mousePosition))
            {
                var genericMenu = new GenericMenu();
                    fillGenericMenu(genericMenu);

                    Rect genericMenuRect = default;
                    genericMenuRect.x = menuButtonRect.x;
                    genericMenuRect.y = menuButtonRect.yMax;
                    genericMenuRect.width = 0f;
                    genericMenuRect.height = 0f;
                    genericMenu.DropDown(genericMenuRect);
                    currentEvent.Use();
            }
            else if(currentEvent.type == EventType.MouseDown && headerRect.Contains(currentEvent.mousePosition) && currentEvent.button == 0)
            {
                componentExpanded = !componentExpanded;
                currentEvent.Use();
            }
        }

        private static void DrawColorTag(Rect titleRect, Color componentColor)
        {
            var colorRect = new Rect
            {
                x = titleRect.xMin,
                y = titleRect.yMin,
                width = 5f,
                height = 17f,
                xMin = 0f,
                xMax = 5f
            };

            EditorGUI.DrawRect(colorRect, componentColor);
        }

        private static void DrawBackground(Rect backgroundRect, bool displayFullHeaderColor, Color componentColor, Color displayColor, out Color backgroundColor)
        {
            Color headerBackgroundColor;
            // Background - if color is white we draw the default color
            if (!displayFullHeaderColor)
            {
                headerBackgroundColor = HeaderBackground;
            }
            else
            {
                headerBackgroundColor = componentColor;
            }

            if (displayColor != Color.black)
            {
                headerBackgroundColor = displayColor;
            }

            EditorGUI.DrawRect(backgroundRect, headerBackgroundColor);
            backgroundColor = headerBackgroundColor;
        }

        private static void DrawDelayInformation(Rect titleRect, string delay, Color backgroundColor)
        {
            var timingRect = new Rect
            {
                x = titleRect.xMax - _timingRectWidth,
                y = titleRect.yMin,
                width = _timingRectWidth,
                height = 17f,
                xMin = titleRect.xMax - _timingRectWidth,
                xMax = titleRect.xMax
            };

            timingRect.x -= 23;

            var delayGuiStyle = new GUIStyle()
            {
                normal =
                {
                    textColor = GetColorText(backgroundColor)
                },
                alignment = TextAnchor.MiddleRight
            };
            EditorGUI.LabelField(timingRect, delay, delayGuiStyle);
        }

        private static void DrawReorderButton(Rect mainRect)
        {
            var reorderRect = mainRect;
            reorderRect.xMin -= 11f;
            reorderRect.y += 5f;
            reorderRect.width = 9f;
            reorderRect.height = 9f;

            const float shapeSquares = 3.0f;
            for (int i = 0; i < shapeSquares; i++)
            {
                var workRect = reorderRect;
                workRect.height = 1;
                workRect.y = reorderRect.y + reorderRect.height * (i / shapeSquares);
                EditorGUI.DrawRect(workRect, Reorder);
            }
        }

        private static void DrawMenuButton(Rect titleRect, out Rect menuButtonRect)
        {
            menuButtonRect = titleRect;
            menuButtonRect.x = titleRect.xMax + 4f;
            menuButtonRect.y = titleRect.y + 2;
            menuButtonRect.width = PaneOptionsIcon.width;
            menuButtonRect.height = PaneOptionsIcon.height;
            
            GUI.DrawTexture(menuButtonRect, PaneOptionsIcon);        
        }
        
        private static bool DrawEnabledStateCheckbox(Rect backgroundRect, bool componentEnabled)
        {
            var toggleRect = backgroundRect;
            toggleRect.x += 16f;
            toggleRect.xMin += _headerElementsOffset;
            toggleRect.y += 2f;
            toggleRect.width = 13f;
            toggleRect.height = 13f;

            componentEnabled = GUI.Toggle(toggleRect, componentEnabled, GUIContent.none, SmallTickBox);
            return componentEnabled;
        }

        private static void DrawTitle(Rect mainRect, string title, string displayLabel, Color backgroundColor, out Rect labelRect)
        {
            labelRect = mainRect;
            labelRect.xMin += 32f + _headerElementsOffset;
            labelRect.xMax -= 20f;

            var finalTitle = string.IsNullOrEmpty(displayLabel) ? title : displayLabel;
            var labelGUIStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                normal =
                {
                    textColor = GetColorText(backgroundColor)
                }
            };

            EditorGUI.LabelField(labelRect, finalTitle, labelGUIStyle);
        }

        private static Color GetColorText(Color background)
        { 
            var r = background.r * 255;
            var g = background.g * 255;
            var b = background.b * 255;
            var luminance = (r * 0.299f + g * 0.587f + b * 0.114f) / 255;
            return luminance < 0.5f ? Color.white : Color.black;
        }

        private static bool DrawFoldoutIcon(Rect mainRect, bool componentExpanded)
        {
            var foldoutRect = mainRect;
            foldoutRect.y += 1f;
            foldoutRect.xMin += _headerElementsOffset;
            foldoutRect.width = 13f;
            foldoutRect.height = 13f;
            componentExpanded = GUI.Toggle(foldoutRect, componentExpanded, GUIContent.none, EditorStyles.foldout);
            return componentExpanded;
        }
    }
}