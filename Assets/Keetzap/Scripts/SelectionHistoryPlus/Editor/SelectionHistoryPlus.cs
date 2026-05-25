//-----------------------------------------------------------------------
// SelectionHistoryPlus.cs
//
// Copyright 2026 Social Point SL. All rights reserved.
//
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Keetzap.SelectionHistoryPlus.Editor
{
    public class SelectionHistoryPlus : EditorWindow
    {
        #region Window Entry

        [MenuItem("Window/General/Selection History +")]
        public static void Init()
        {
            SelectionHistoryPlus window = GetWindow<SelectionHistoryPlus>();

            window.autoRepaintOnSceneChange = true;
            window.titleContent.image = EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin ? "d_UnityEditor.SceneHierarchyWindow" : "UnityEditor.SceneHierarchyWindow").image;
            window.titleContent.text = SelectionHistoryPlusLabels.SelectionHistoryPlus;
            window.wantsMouseMove = true;
            window.Show();
        }

        #endregion

        #region Properties & Fields

        private string _iconPrefix => EditorGUIUtility.isProSkin ? "d_" : "";
        private AnimBool _settingAnimation;
        private bool _settingExpanded;
        private AnimBool _clearAnimation;
        private bool _historyVisible = true;
        private bool _clearRequested;

        private List<Object> _SelectionHistoryPlus = new();
        private readonly SelectionHistoryPlusPinStore _pinStore = new();
        private static bool _muteRecording;
        private Vector2 scrollPos;
        private SelectionHistoryPlusIconSet _iconSet;
        private Rect _configButtonRect;
        private Rect _helpButtonRect;

        [NonSerialized] private bool _isRecording;

        private readonly HashSet<Object> _multiSelection = new();
        private int _lastClickedIndex = -1;
        private bool _ignoreNextLostFocus;

        #endregion

        #region Unity Lifecycle

        private void OnEnable()
        {
            _isRecording = SelectionHistoryPlusPrefs.StartRecordingOnOpen;
            _settingAnimation = new AnimBool(false);
            _settingAnimation.valueChanged.AddListener(this.Repaint);
            _settingAnimation.speed = 4f;
            _clearAnimation = new AnimBool(false);
            _clearAnimation.valueChanged.AddListener(this.Repaint);
            _clearAnimation.speed = _settingAnimation.speed;

            if (_iconSet == null)
            {
                _iconSet = SelectionHistoryPlusIconSet.Load();
            }

            _pinStore.Load();
            LoadHistory();
        }

        private void OnDisable()
        {
            _pinStore.Save();
            SaveHistory();
        }

        private void OnFocus()
        {
            _SelectionHistoryPlus = _SelectionHistoryPlus.Where(x => x != null).ToList();
        }

        private void OnLostFocus()
        {
            if (_ignoreNextLostFocus)
            {
                _ignoreNextLostFocus = false;
                return;
            }

            _multiSelection.Clear();
            Repaint();
        }

        private void OnSelectionChange()
        {
            if (!_muteRecording && EditorWindow.focusedWindow != this)
            {
                _multiSelection.Clear();
                foreach (var obj in Selection.objects)
                {
                    if (obj != null && _SelectionHistoryPlus.Contains(obj))
                    {
                        _multiSelection.Add(obj);
                    }
                }
            }

            this.Repaint();

            if (_muteRecording || !Selection.activeObject)
            {
                return;
            }

            if (!SelectionHistoryPlusTypeFilter.Passes(Selection.activeObject))
            {
                return;
            }

            AddToHistory();
        }

        #endregion

        #region UI Entry Point

        private void OnGUI()
        {
            DrawToolBar();
            DrawSettingsPanel();
            DrawHistoryList();
        }

        private void DrawToolBar()
        {
            var toolbarWidth = 32f;

            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                GUIContent recONContent = GetGUIContent(_iconSet.RecordingON, "●", "Recording (click to stop)", false);
                GUIContent recOFFContent = GetGUIContent(_iconSet.RecordingOFF, "○", "Not recording (click to start)", false);
                GUIContent recordingContent = _isRecording ? recONContent : recOFFContent;

                if (GUILayout.Button(recordingContent, EditorStyles.toolbarButton, GUILayout.Width(32f)))
                {
                    _isRecording = !_isRecording;
                    Repaint();
                }

                using (new EditorGUI.DisabledScope(_SelectionHistoryPlus.Count == 0))
                {
                    GUIContent filterContent = GetGUIContent(_iconSet.Filter, "FilterByType", $"Sorting: {SelectionHistoryPlusPrefs.Sorting}");

                    if (GUILayout.Button(filterContent, EditorStyles.toolbarButton, GUILayout.Width(toolbarWidth)))
                    {
                        Rect r = GUILayoutUtility.GetLastRect();
                        PopupWindow.Show(r, new SelectionHistoryPlusTypeFilterPopup(this));
                    }

                    GUIContent trashContent = GetGUIContent(_iconSet.Trash, "TreeEditor.Trash", "Clear history");

                    if (GUILayout.Button(trashContent, EditorStyles.toolbarButton, GUILayout.Width(toolbarWidth)))
                    {
                        _clearRequested = true;
                        _historyVisible = false;
                    }

                    GUIContent orderContent = GetGUIContent(_iconSet.Order, "CustomSorting", "Sorted by...");

                    if (EditorGUILayout.DropdownButton(orderContent, FocusType.Passive, EditorStyles.toolbarDropDown, GUILayout.Width(40f)))
                    {
                        var menu = new GenericMenu();

                        void AddSort(SelectionHistoryPlusPrefs.SortMode mode, string label)
                        {
                            bool on = SelectionHistoryPlusPrefs.Sorting == mode;
                            menu.AddItem(new GUIContent(label), on, () =>
                            {
                                SelectionHistoryPlusPrefs.Sorting = mode;
                                Repaint();
                            });
                        }

                        AddSort(SelectionHistoryPlusPrefs.SortMode.History, "History");
                        AddSort(SelectionHistoryPlusPrefs.SortMode.Alphabetically, "Alphabetically");
                        AddSort(SelectionHistoryPlusPrefs.SortMode.Type, "Type");

                        menu.DropDown(GUILayoutUtility.GetLastRect());
                    }

                    GUIContent pinnedOnTopContent = GetGUIContent(_iconSet.PinnedOnTop, "Favorite Icon", "Show pinned on top");
                    bool newPinnedOnTop = GUILayout.Toggle(SelectionHistoryPlusPrefs.PinnedOnTop, pinnedOnTopContent, EditorStyles.toolbarButton, GUILayout.Width(toolbarWidth));

                    if (newPinnedOnTop != SelectionHistoryPlusPrefs.PinnedOnTop)
                    {
                        SelectionHistoryPlusPrefs.PinnedOnTop = newPinnedOnTop;
                        Repaint();
                    }

                    GUIContent dontClearContent = GetGUIContent(_iconSet.KeepPinnedOnClear, "clear", "Don't remove pinned on clear");
                    bool newDontClear = GUILayout.Toggle(SelectionHistoryPlusPrefs.KeepPinnedOnClear, dontClearContent, EditorStyles.toolbarButton, GUILayout.Width(toolbarWidth));

                    if (newDontClear != SelectionHistoryPlusPrefs.KeepPinnedOnClear)
                    {
                        SelectionHistoryPlusPrefs.KeepPinnedOnClear = newDontClear;
                        Repaint();
                    }

                }

                GUILayout.FlexibleSpace();

                GUIContent btnConfigContent = GetGUIContent(_iconSet.Buttons, "Preset.Context", "Configure Buttons");

                if (EditorGUILayout.DropdownButton(btnConfigContent, FocusType.Passive, EditorStyles.toolbarButton, GUILayout.Width(toolbarWidth)))
                {
                    PopupWindow.Show(_configButtonRect, new SelectionHistoryPlusButtonPopup(this));
                }

                if (Event.current.type == EventType.Repaint)
                {
                    _configButtonRect = GUILayoutUtility.GetLastRect();
                }

                GUIContent settingsContent = GetGUIContent(_iconSet.Settings, "Settings", "Edit settings");
                _settingExpanded = GUILayout.Toggle(_settingExpanded, settingsContent, EditorStyles.toolbarButton, GUILayout.Width(toolbarWidth));
                _settingAnimation.target = _settingExpanded;

                GUIContent helpContent = GetGUIContent(_iconSet.Help, "_Help", "Help & Support");

                if (EditorGUILayout.DropdownButton(helpContent, FocusType.Passive, EditorStyles.toolbarButton, GUILayout.Width(toolbarWidth)))
                {
                    PopupWindow.Show(_helpButtonRect, new SelectionHistoryPlusHelpPopup());
                }

                if (Event.current.type == EventType.Repaint)
                {
                    _helpButtonRect = GUILayoutUtility.GetLastRect();
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private GUIContent GetGUIContent(Texture2D texture2D, string alternativeContent, string tooltip, bool icon = true)
        {
            GUIContent content = texture2D != null ? new GUIContent(texture2D) : icon ? EditorGUIUtility.IconContent($"{_iconPrefix}{alternativeContent}") : new GUIContent(alternativeContent);
            content.tooltip = tooltip;

            return content;
        }

        private void DrawSettingsPanel()
        {
            if (EditorGUILayout.BeginFadeGroup(_settingAnimation.faded))
            {
                EditorGUILayout.Space();
                var backgroundSettings = EditorGUILayout.BeginVertical();
                {
                    EditorGUILayout.LabelField("Record", EditorStyles.boldLabel);

                    EditorGUI.indentLevel++;
                    {
                        SelectionHistoryPlusPrefs.StartRecordingOnOpen = EditorGUILayout.ToggleLeft("Start recording when opening", SelectionHistoryPlusPrefs.StartRecordingOnOpen);

                        using (new EditorGUILayout.HorizontalScope())
                        {
                            SelectionHistoryPlusPrefs.RecordHierarchy = EditorGUILayout.ToggleLeft("Hierarchy", SelectionHistoryPlusPrefs.RecordHierarchy, GUILayout.MaxWidth(100f));
                            SelectionHistoryPlusPrefs.RecordProject = EditorGUILayout.ToggleLeft("Project window", SelectionHistoryPlusPrefs.RecordProject);
                        }
                    }
                    EditorGUI.indentLevel--;
                    EditorGUILayout.Space();

                    EditorGUILayout.LabelField("History", EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;
                    {
                        int historySize = SelectionHistoryPlusPrefs.MaxHistorySize;

                        using (new EditorGUILayout.HorizontalScope())
                        {
                            EditorGUILayout.LabelField("Max Size", GUILayout.Width(80f));
                            historySize = Mathf.RoundToInt(GUILayout.HorizontalSlider(historySize, 10f, 200f, GUILayout.Width(100f)));
                            EditorGUILayout.LabelField($"{Mathf.RoundToInt(historySize)}", GUILayout.Width(40f));
                        }

                        SelectionHistoryPlusPrefs.MaxHistorySize = historySize;

                        EditorGUILayout.Space(4);
                        EditorGUILayout.LabelField("When reaching the max size:");

                        EditorGUI.indentLevel++;
                        {
                            SelectionHistoryPlusPrefs.DontRemovePinnedOnLimit = EditorGUILayout.ToggleLeft("Don't remove pinned objects", SelectionHistoryPlusPrefs.DontRemovePinnedOnLimit);

                            bool isDoNothing = SelectionHistoryPlusPrefs.HistoryLimitAction == SelectionHistoryPlusPrefs.HistoryLimitBehavior.DoNothing;
                            bool isRemoveOldest = SelectionHistoryPlusPrefs.HistoryLimitAction == SelectionHistoryPlusPrefs.HistoryLimitBehavior.RemoveOldest;

                            using (new EditorGUILayout.HorizontalScope())
                            {
                                EditorGUILayout.LabelField("", GUILayout.Width(24f));

                                if (GUILayout.Toggle(isRemoveOldest, new GUIContent("  Remove oldest"), "radio", GUILayout.Width(120f)) && !isRemoveOldest)
                                {
                                    SelectionHistoryPlusPrefs.HistoryLimitAction = SelectionHistoryPlusPrefs.HistoryLimitBehavior.RemoveOldest;
                                }
                                if (GUILayout.Toggle(isDoNothing, new GUIContent("  Don't add anything"), "radio", GUILayout.Width(160f)) && !isDoNothing)
                                {
                                    SelectionHistoryPlusPrefs.HistoryLimitAction = SelectionHistoryPlusPrefs.HistoryLimitBehavior.DoNothing;
                                }
                            }
                        }
                        EditorGUI.indentLevel--;
                    }
                    EditorGUI.indentLevel--;
                    EditorGUILayout.Space();

                    EditorGUILayout.LabelField("Display", EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;
                    {
                        float rowHeight = SelectionHistoryPlusPrefs.RowHeight;

                        using (new EditorGUILayout.HorizontalScope())
                        {
                            EditorGUILayout.LabelField("Row height", GUILayout.Width(80f));
                            rowHeight = GUILayout.HorizontalSlider(rowHeight, 16f, 32f, GUILayout.Width(100f));
                            EditorGUILayout.LabelField($"{Mathf.RoundToInt(rowHeight)}", GUILayout.Width(40f));
                        }

                        SelectionHistoryPlusPrefs.RowHeight = rowHeight;
                    }
                    EditorGUI.indentLevel--;
                    EditorGUILayout.Space();
                }
                EditorGUILayout.EndVertical();

                var backgroundSettingsColor = EditorGUIUtility.isProSkin ? new Color(0f, 0f, 0f, 0.2f) : new Color(0f, 0f, 0f, 0.08f);

                EditorGUI.DrawRect(backgroundSettings, backgroundSettingsColor);
            }

            EditorGUILayout.EndFadeGroup();
        }

        private void DrawHistoryList()
        {
            _clearAnimation.target = !_historyVisible;

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.MaxHeight(this.maxSize.y - 20f));
            {
                EditorGUILayout.BeginFadeGroup(1f - _clearAnimation.faded);

                var prevColor = GUI.color;
                var prevBgColor = GUI.backgroundColor;

                var items = _SelectionHistoryPlus.Where(x => x != null);

                List<Object> ordered = _SelectionHistoryPlus.Where(x => x != null).ToList();

                if (SelectionHistoryPlusPrefs.PinnedOnTop)
                {
                    var pinned = ordered.Where(x => _pinStore.IsPinned(x));
                    var unpinned = ordered.Where(x => !_pinStore.IsPinned(x));
                    ordered = pinned.Concat(unpinned).ToList();
                }

                var all = _SelectionHistoryPlus.Where(x => x != null);

                if (SelectionHistoryPlusPrefs.PinnedOnTop)
                {
                    var pinned = all.Where(x => _pinStore.IsPinned(x));
                    var unpinned = all.Where(x => !_pinStore.IsPinned(x));

                    if (SelectionHistoryPlusPrefs.Sorting != SelectionHistoryPlusPrefs.SortMode.History)
                    {
                        pinned = SortHistory(pinned);
                        unpinned = SortHistory(unpinned);
                    }

                    var pinnedList = pinned.ToList();
                    var unpinnedList = unpinned.ToList();

                    int row = 0;

                    if (pinnedList.Count > 0)
                    {
                        EditorGUILayout.Space(2);
                        EditorGUILayout.LabelField("Pinned", EditorStyles.boldLabel);
                        EditorGUILayout.Space(2);

                        for (int i = 0; i < pinnedList.Count; i++)
                        {
                            DrawHistoryRow(pinnedList[i], row++);
                        }

                        EditorGUILayout.Space(4);
                        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                        EditorGUILayout.Space(4);
                    }

                    EditorGUILayout.LabelField("History", EditorStyles.boldLabel);
                    EditorGUILayout.Space(2);

                    for (int i = 0; i < unpinnedList.Count; i++)
                    {
                        DrawHistoryRow(unpinnedList[i], row++);
                    }
                }
                else
                {
                    var list = all;

                    if (SelectionHistoryPlusPrefs.Sorting != SelectionHistoryPlusPrefs.SortMode.History)
                    {
                        list = SortHistory(list);
                    }

                    var finalList = list.ToList();

                    for (int i = 0; i < finalList.Count; i++)
                    {
                        DrawHistoryRow(finalList[i], i);
                    }
                }

                if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && GUIUtility.hotControl == 0)
                {
                    Rect listRect = GUILayoutUtility.GetLastRect();

                    _multiSelection.Clear();
                    _lastClickedIndex = -1;
                    Repaint();
                    Event.current.Use();
                }

                EditorGUILayout.EndFadeGroup();
            }
            EditorGUILayout.EndScrollView();

            if (_clearRequested && _clearAnimation.faded == 1f)
            {
                if (SelectionHistoryPlusPrefs.KeepPinnedOnClear)
                {
                    _SelectionHistoryPlus = _SelectionHistoryPlus.Where(x => x != null && _pinStore.IsPinned(x)).ToList();
                }
                else
                {
                    _SelectionHistoryPlus.Clear();
                }

                _clearRequested = false;
                _historyVisible = true;
            }

            if (_SelectionHistoryPlus.Count == 0)
            {
                _historyVisible = true;
            }
        }
        #endregion

        #region UI Components (Rows, Menus)
        private void DrawHistoryRow(Object obj, int rowIndex)
        {
            float btnHeight = SelectionHistoryPlusPrefs.RowHeight;
            float btnWidth = btnHeight * 1.4f;
            float btnSpacing = 2f;

            int visibleButtonCount = 0;
            if (SelectionHistoryPlusPrefs.ShowRevealButton) visibleButtonCount++;
            if (SelectionHistoryPlusPrefs.ShowDeleteButton) visibleButtonCount++;
            if (SelectionHistoryPlusPrefs.ShowInspectorButton) visibleButtonCount++;
            if (SelectionHistoryPlusPrefs.ShowPinButton) visibleButtonCount++;

            float totalButtonsWidth = visibleButtonCount * btnWidth + (visibleButtonCount > 0 ? (visibleButtonCount - 1) * btnSpacing : 0);

            Rect rect = GUILayoutUtility.GetRect(0, btnHeight, GUILayout.ExpandWidth(true));

            var prevColor = GUI.color;
            var prevBgColor = GUI.backgroundColor;

            GUI.color = rowIndex % 2 == 0 ? Color.grey * (EditorGUIUtility.isProSkin ? 1f : 1.7f) : Color.grey * (EditorGUIUtility.isProSkin ? 1.05f : 1.66f);

            bool isMultiSelected = _multiSelection.Contains(obj);

            var e = Event.current;
            if (e.type == EventType.MouseDown && e.button == 1 && rect.Contains(e.mousePosition))
            {
                if (!_multiSelection.Contains(obj))
                {
                    _multiSelection.Clear();
                    _multiSelection.Add(obj);
                    _ignoreNextLostFocus = true;
                    Repaint();
                }

                ShowContextMenuFor(obj, rowIndex);
                e.Use();
            }

            bool isHover = rect.Contains(Event.current.mousePosition);
            bool isMultiSelectionActive = _multiSelection.Count > 1;
            if (isMultiSelected)
            {
                if (isMultiSelectionActive)
                {
                    GUI.color = new Color(0.48f, 0.66f, 0.8f, 1f);
                }
                else
                {
                    GUI.color = new Color(0.43f, 0.61f, 0.78f, 1f);
                }
            }
            else if (isHover)
            {
                GUI.color = Color.grey * 1.3f;
            }

            Event evt = Event.current;
            if (evt.type == EventType.MouseDown && evt.clickCount == 2 && rect.Contains(evt.mousePosition))
            {
                OpenInSceneIfPossible(obj);
                evt.Use();
            }

            EditorGUI.DrawRect(rect, GUI.color);

            GUI.color = prevColor;
            GUI.backgroundColor = prevBgColor;

            Rect mainAreaRect = rect;
            mainAreaRect.width = rect.width - totalButtonsWidth - (visibleButtonCount > 0 ? btnSpacing : 0);

            Texture icon = EditorGUIUtility.ObjectContent(obj, obj.GetType()).image;
            float iconSize = btnHeight;
            float iconPadding = 2f;

            if (icon != null)
            {
                Rect iconRect = new Rect(mainAreaRect.x + iconPadding, mainAreaRect.y + (mainAreaRect.height - iconSize) / 2f, iconSize, iconSize);
                GUI.DrawTexture(iconRect, icon, ScaleMode.ScaleToFit);
            }

            Rect textRect = mainAreaRect;
            textRect.x += iconSize + iconPadding * 2;
            textRect.width -= iconSize + iconPadding * 2;

            EditorGUI.LabelField(textRect, obj.name, EditorStyles.label);

            if (e.type == EventType.MouseDrag && e.button == 0 && mainAreaRect.Contains(e.mousePosition))
            {
                var dragTargets = _multiSelection.Count > 1 && _multiSelection.Contains(obj) ? _multiSelection.ToArray() : new Object[] { obj };

                DragAndDrop.PrepareStartDrag();
                DragAndDrop.objectReferences = dragTargets;
                DragAndDrop.StartDrag(dragTargets.Length > 1 ? $"{dragTargets.Length} objects" : obj.name);
                e.Use();
            }

            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && mainAreaRect.Contains(Event.current.mousePosition))
            {
                if (Event.current.control)
                {
                    if (_multiSelection.Contains(obj))
                    {
                        _multiSelection.Remove(obj);
                    }
                    else
                    {
                        _multiSelection.Add(obj);
                    }
                }
                else if (Event.current.shift && _lastClickedIndex >= 0)
                {
                    int start = Mathf.Min(_lastClickedIndex, rowIndex);
                    int end = Mathf.Max(_lastClickedIndex, rowIndex);

                    var currentList = GetCurrentDisplayList();
                    for (int i = start; i <= end && i < currentList.Count; i++)
                    {
                        if (currentList[i] != null)
                        {
                            _multiSelection.Add(currentList[i]);
                        }
                    }
                }
                else
                {
                    _multiSelection.Clear();
                    _multiSelection.Add(obj);
                    _ignoreNextLostFocus = true;
                    RevealObject(obj);
                }

                _lastClickedIndex = rowIndex;
                Event.current.Use();
                Repaint();
            }

            float buttonX = rect.xMax - totalButtonsWidth;

            if (SelectionHistoryPlusPrefs.ShowRevealButton)
            {
                Rect btnRect = new Rect(buttonX, rect.y, btnWidth, btnHeight);
                GUIContent revealContent = _iconSet.ShowOnBrowser != null ? new GUIContent(_iconSet.ShowOnBrowser) : EditorGUIUtility.IconContent(_iconPrefix + "FolderOpened Icon");
                revealContent.tooltip = GetRevealInFileBrowserLabel();

                if (GUI.Button(btnRect, revealContent))
                {
                    var targets = _multiSelection.Count > 1 && _multiSelection.Contains(obj) ? _multiSelection.ToList() : new List<Object> { obj };

                    foreach (var target in targets)
                    {
                        RevealInExplorerOrFinder(target);
                    }
                }

                buttonX += btnWidth + btnSpacing;
            }

            if (SelectionHistoryPlusPrefs.ShowDeleteButton)
            {
                Rect btnRect = new Rect(buttonX, rect.y, btnWidth, btnHeight);
                GUIContent deleteContent = GetGUIContent(_iconSet.Trash, "TreeEditor.Trash", SelectionHistoryPlusLabels.DeleteFromHistory);

                if (GUI.Button(btnRect, deleteContent))
                {
                    var targets = _multiSelection.Count > 1 && _multiSelection.Contains(obj) ? _multiSelection.ToList() : new List<Object> { obj };

                    foreach (var target in targets)
                    {
                        RemoveFromHistory(target);
                    }

                    _multiSelection.Clear();
                    GUIUtility.ExitGUI();
                }

                buttonX += btnWidth + btnSpacing;
            }

            if (SelectionHistoryPlusPrefs.ShowInspectorButton)
            {
                Rect btnRect = new Rect(buttonX, rect.y, btnWidth, btnHeight);
                GUIContent showInInspectorContent = GetGUIContent(_iconSet.ShowOnInspector, "UnityEditor.InspectorWindow", SelectionHistoryPlusLabels.ShowInInspector);

                bool multipleSelected = _multiSelection.Count > 1;
                using (new EditorGUI.DisabledScope(multipleSelected))
                {
                    if (GUI.Button(btnRect, showInInspectorContent))
                    {
                        ShowInInspector(obj, rowIndex);
                    }
                }

                buttonX += btnWidth + btnSpacing;
            }

            if (SelectionHistoryPlusPrefs.ShowPinButton)
            {
                Rect btnRect = new Rect(buttonX, rect.y, btnWidth, btnHeight);

                bool pinned = _pinStore.IsPinned(obj);
                GUIContent pinContent = GetGUIContent(_iconSet.UnpinSelection, "●", $"{SelectionHistoryPlusLabels.Unpin} selection", false);
                GUIContent unpinContent = GetGUIContent(_iconSet.PinSelection, "○", $"{SelectionHistoryPlusLabels.Pin} selection", false);
                GUIContent pinUnpinContent = pinned ? pinContent : unpinContent;

                if (GUI.Button(btnRect, pinUnpinContent))
                {
                    var targets = _multiSelection.Count > 1 && _multiSelection.Contains(obj) ? _multiSelection.ToList() : new List<Object> { obj };

                    bool shouldPin = !pinned;

                    foreach (var target in targets)
                    {
                        if (shouldPin && !_pinStore.IsPinned(target))
                        {
                            _pinStore.Pin(target);
                        }
                        else if (!shouldPin && _pinStore.IsPinned(target))
                        {
                            _pinStore.Unpin(target);
                        }
                    }

                    Repaint();
                }
            }
        }

        private void ShowContextMenuFor(Object obj, int index)
        {
            if (obj == null)
            {
                return;
            }

            var targets = _multiSelection.Count > 1 && _multiSelection.Contains(obj) ? _multiSelection.ToList() : new List<Object> { obj };
            int count = targets.Count;

            var menu = new GenericMenu();

            string deleteLabel = count > 1 ? $"Delete {count} items from history" : SelectionHistoryPlusLabels.DeleteFromHistory;
            menu.AddItem(new GUIContent(deleteLabel), false, () =>
            {
                foreach (var t in targets)
                {
                    RemoveFromHistory(t);
                }

                _multiSelection.Clear();
            });

            menu.AddSeparator("");

            if (count == 1)
            {
                menu.AddItem(new GUIContent(SelectionHistoryPlusLabels.ShowInInspector), false, () => ShowInInspector(obj, index));

                if (PrefabUtility.IsPartOfPrefabAsset(obj))
                {
                    menu.AddItem(new GUIContent(SelectionHistoryPlusLabels.OpenPrefab), false, () => OpenInSceneIfPossible(obj));
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent(SelectionHistoryPlusLabels.OpenPrefab));
                }
            }
            else
            {
                menu.AddDisabledItem(new GUIContent(SelectionHistoryPlusLabels.ShowInInspector));
                menu.AddDisabledItem(new GUIContent(SelectionHistoryPlusLabels.OpenPrefab));
            }

            bool isPinned = _pinStore.IsPinned(obj); string pinLabel = isPinned ? SelectionHistoryPlusLabels.Unpin : SelectionHistoryPlusLabels.Pin;
            if (count > 1)
            {
                pinLabel += " selected";
            }

            menu.AddItem(new GUIContent(pinLabel), false, () =>
            {
                bool shouldPin = !isPinned;
                foreach (var t in targets)
                {
                    if (shouldPin && !_pinStore.IsPinned(t))
                    {
                        _pinStore.Pin(t);
                    }
                    else if (!shouldPin && _pinStore.IsPinned(t))
                    {
                        _pinStore.Unpin(t);
                    }
                }
                Repaint();
            });

            menu.AddSeparator("");

            menu.AddItem(new GUIContent(GetRevealInFileBrowserLabel()), false, () =>
            {
                foreach (var t in targets)
                {
                    RevealInExplorerOrFinder(t);
                }
            });

            menu.ShowAsContext();
        }

        public static string GetRevealInFileBrowserLabel()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.OSXEditor:
                    return "Reveal in Finder";

                case RuntimePlatform.WindowsEditor:
                    return "Show in Explorer";

                default:
                    return "Reveal in File Browser";
            }
        }
        #endregion

        #region Actions / Commands
        private void AddToHistory()
        {
            if (!_isRecording)
            {
                return;
            }

            if (Selection.activeObject.GetType() == typeof(UnityEditor.DefaultAsset))
            {
                return;
            }

            if (EditorUtility.IsPersistent(Selection.activeObject) && !SelectionHistoryPlusPrefs.RecordProject)
            {
                return;
            }

            if (_SelectionHistoryPlus.Contains(Selection.activeObject))
            {
                return;
            }

            if (EditorUtility.IsPersistent(Selection.activeObject) == false && !SelectionHistoryPlusPrefs.RecordHierarchy)
            {
                return;
            }

            if (_SelectionHistoryPlus.Count >= SelectionHistoryPlusPrefs.MaxHistorySize)
            {
                if (SelectionHistoryPlusPrefs.HistoryLimitAction == SelectionHistoryPlusPrefs.HistoryLimitBehavior.DoNothing)
                {
                    return;
                }

                bool removed = false;

                for (int i = _SelectionHistoryPlus.Count - 1; i >= 0; i--)
                {
                    var obj = _SelectionHistoryPlus[i];
                    if (obj == null)
                    {
                        _SelectionHistoryPlus.RemoveAt(i);
                        removed = true;
                        break;
                    }

                    if (SelectionHistoryPlusPrefs.DontRemovePinnedOnLimit && _pinStore.IsPinned(obj))
                    {
                        continue;
                    }

                    _SelectionHistoryPlus.RemoveAt(i);
                    removed = true;
                    break;
                }

                if (!removed)
                {
                    return;
                }
            }

            _SelectionHistoryPlus.Insert(0, Selection.activeObject);
        }

        private void RemoveFromHistory(Object obj)
        {
            if (obj == null)
            {
                return;
            }

            _SelectionHistoryPlus.RemoveAll(x => x == obj);
            _pinStore.Unpin(obj);

            Repaint();
        }

        private void SetSelection(Object target, int index)
        {
            _muteRecording = true;
            Selection.activeObject = target;

            _multiSelection.Clear();
            _multiSelection.Add(target);

            EditorGUIUtility.PingObject(target);
            _muteRecording = false;
        }

        private static void RevealObject(Object target)
        {
            if (target == null)
            {
                return;
            }

            string path = AssetDatabase.GetAssetPath(target);
            if (!string.IsNullOrEmpty(path))
            {
                EditorUtility.FocusProjectWindow();
                EditorGUIUtility.PingObject(target);
                return;
            }

            EditorGUIUtility.PingObject(target);
        }

        private void ShowInInspector(Object target, int index)
        {
            _ignoreNextLostFocus = true;
            SetSelection(target, index);
            EditorApplication.delayCall += FocusInspectorWindow;
        }

        private static void OpenInSceneIfPossible(Object target)
        {
            if (target == null)
            {
                return;
            }

            if (PrefabUtility.IsPartOfPrefabAsset(target) || target is SceneAsset)
            {
                AssetDatabase.OpenAsset(target);
            }
        }

        private static void RevealInExplorerOrFinder(Object obj)
        {
            string path = AssetDatabase.GetAssetPath(obj);

            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            EditorUtility.RevealInFinder(path);
        }

        private void SaveHistory()
        {
            var ids = new HashSet<string>();
            foreach (var obj in _SelectionHistoryPlus)
            {
                if (obj == null) continue;
                string id = SelectionHistoryPlusPinStore.GetObjectId(obj);
                if (!string.IsNullOrEmpty(id))
                {
                    ids.Add(id);
                }
            }

            var validIds = new List<string>();
            foreach (var obj in _SelectionHistoryPlus)
            {
                if (obj == null) continue;
                string id = SelectionHistoryPlusPinStore.GetObjectId(obj);
                if (!string.IsNullOrEmpty(id) && !validIds.Contains(id))
                {
                    validIds.Add(id);
                }
            }

            EditorPrefs.SetString(SelectionHistoryPlusPrefs.GetHistoryPrefsKey(), string.Join(";", validIds));
        }

        private void LoadHistory()
        {
            string raw = EditorPrefs.GetString(SelectionHistoryPlusPrefs.GetHistoryPrefsKey(), string.Empty);
            if (string.IsNullOrEmpty(raw))
            {
                return;
            }

            var loaded = new List<Object>();
            var ids = raw.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string id in ids)
            {
                Object obj = SelectionHistoryPlusPinStore.ResolveIdToObject(id);
                if (obj != null && !loaded.Contains(obj))
                {
                    loaded.Add(obj);
                }
            }

            _SelectionHistoryPlus = loaded;
        }
        #endregion

        #region Internal Helpers
        private static void FocusInspectorWindow()
        {
            var inspectorType = Type.GetType("UnityEditor.InspectorWindow,UnityEditor");
            if (inspectorType != null)
            {
                EditorWindow.FocusWindowIfItsOpen(inspectorType);
            }
        }

        private List<Object> GetCurrentDisplayList()
        {
            var all = _SelectionHistoryPlus.Where(x => x != null);

            if (SelectionHistoryPlusPrefs.PinnedOnTop)
            {
                var pinned = all.Where(x => _pinStore.IsPinned(x));
                var unpinned = all.Where(x => !_pinStore.IsPinned(x));

                if (SelectionHistoryPlusPrefs.Sorting != SelectionHistoryPlusPrefs.SortMode.History)
                {
                    pinned = SortHistory(pinned);
                    unpinned = SortHistory(unpinned);
                }

                return pinned.Concat(unpinned).ToList();
            }
            else
            {
                var list = all;

                if (SelectionHistoryPlusPrefs.Sorting != SelectionHistoryPlusPrefs.SortMode.History)
                {
                    list = SortHistory(list);
                }

                return list.ToList();
            }
        }
        #endregion

        #region Sorting
        private static IEnumerable<Object> SortHistory(IEnumerable<Object> list)
        {
            return SortHistory(list, SelectionHistoryPlusPrefs.Sorting);
        }

        private static IEnumerable<Object> SortHistory(IEnumerable<Object> list, SelectionHistoryPlusPrefs.SortMode mode)
        {
            switch (mode)
            {
                case SelectionHistoryPlusPrefs.SortMode.Alphabetically:
                    return list.OrderBy(o => o != null ? o.name : string.Empty, StringComparer.OrdinalIgnoreCase);

                case SelectionHistoryPlusPrefs.SortMode.Type:
                    return list
                        .OrderBy(o => o != null ? o.GetType().Name : string.Empty, StringComparer.OrdinalIgnoreCase)
                        .ThenBy(o => o != null ? o.name : string.Empty, StringComparer.OrdinalIgnoreCase);

                case SelectionHistoryPlusPrefs.SortMode.History:
                default:
                    return list;
            }
        }
        #endregion
    }
}
