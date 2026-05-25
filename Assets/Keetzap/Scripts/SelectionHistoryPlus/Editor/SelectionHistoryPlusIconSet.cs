//-----------------------------------------------------------------------
// SelectionHistoryPlusIconSet.cs
//
// Copyright 2026 Social Point SL. All rights reserved.
//
//-----------------------------------------------------------------------
using UnityEditor;
using UnityEngine;

namespace Keetzap.SelectionHistoryPlus.Editor
{
    [CreateAssetMenu(fileName = "SelectionHistoryPlusIconSet", menuName = "Tools/Selection History/Icon Set", order = 0)]
    public sealed class SelectionHistoryPlusIconSet : ScriptableObject
    {
        [Header("Toolbar Icons")]
        [SerializeField] private Texture2D _filter;
        [SerializeField] private Texture2D _trash;
        [SerializeField] private Texture2D _settings;

        [Header("Toggles")]
        [SerializeField] private Texture2D _keepPinnedOnClear;
        [SerializeField] private Texture2D _pinnedOnTop;
        [SerializeField] private Texture2D _showOnInspector;
        [SerializeField] private Texture2D _pinSelection;
        [SerializeField] private Texture2D _unpinSelection;
        [SerializeField] private Texture2D _recordingON;
        [SerializeField] private Texture2D _recordingOFF;
        [SerializeField] private Texture2D _showOnBrowser;

        [Header("Menus")]
        [SerializeField] private Texture2D _order;
        [SerializeField] private Texture2D _buttons;
        [SerializeField] private Texture2D _help;

        public Texture2D Filter => _filter;
        public Texture2D Trash => _trash;
        public Texture2D Settings => _settings;
        public Texture2D KeepPinnedOnClear => _keepPinnedOnClear;
        public Texture2D PinnedOnTop => _pinnedOnTop;
        public Texture2D ShowOnInspector => _showOnInspector;
        public Texture2D PinSelection => _pinSelection;
        public Texture2D UnpinSelection => _unpinSelection;
        public Texture2D RecordingON => _recordingON;
        public Texture2D RecordingOFF => _recordingOFF;
        public Texture2D ShowOnBrowser => _showOnBrowser;
        public Texture2D Order => _order;
        public Texture2D Buttons => _buttons;
        public Texture2D Help => _help;

        private static SelectionHistoryPlusIconSet _instance;

        public static SelectionHistoryPlusIconSet Load()
        {
            if (_instance != null)
            {
                return _instance;
            }

            string[] guids = AssetDatabase.FindAssets("t:SelectionHistoryPlusIconSet");
            if (guids == null || guids.Length == 0)
            {
                return null;
            }

            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            _instance = AssetDatabase.LoadAssetAtPath<SelectionHistoryPlusIconSet>(path);

            return _instance;
        }
    }
}
