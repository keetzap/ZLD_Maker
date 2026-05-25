//-----------------------------------------------------------------------
// SelectionHistoryPlusPrefs.cs
//
// Copyright 2026 Social Point SL. All rights reserved.
//
//-----------------------------------------------------------------------
using System;
using UnityEditor;
using UnityEngine;

namespace Keetzap.SelectionHistoryPlus.Editor
{
    public static class SelectionHistoryPlusPrefs
    {
        private const string PinnedPrefsKeySuffix = "PinnedIds";
        private const string RecordHierarchyKey = "RecordHierarchy";
        private const string RecordProjectKey = "RecordProject";
        private const string MaxHistorySizeKey = "MaxHistorySize";
        private const string RowHeightKey = "RowHeight";
        private const string KeepPinnedOnClearKey = "KeepPinnedOnClear";
        private const string PinnedOnTopKey = "PinnedOnTop";
        private const string SortingKey = "Sorting";
        private const string StartRecordingOnOpenKey = "StartRecordingOnOpen";
        private const string TypeFilterMaskKey = "TypeFilterMask";
        private const string HistoryIdsKey = "HistoryIds";
        private const string ShowPinButtonKey = "ShowPinButton";
        private const string ShowInspectorButtonKey = "ShowInspectorButton";
        private const string ShowDeleteButtonKey = "ShowDeleteButton";
        private const string ShowRevealButtonKey = "ShowRevealButton";
        private const string DontRemovePinnedOnLimitKey = "DontRemovePinnedOnLimit";
        private const string HistoryLimitActionKey = "HistoryLimitAction";

        internal static string GetHistoryPrefsKey() => GetKey(HistoryIdsKey);

        public enum SortMode
        {
            History = 0,
            Alphabetically = 1,
            Type = 2
        }

        public enum HistoryLimitBehavior
        {
            DoNothing = 0,
            RemoveOldest = 1
        }

        [Flags]
        public enum TypeFilterMask
        {
            SceneObjects = 1 << 0,
            Prefabs = 1 << 1,
            Scenes = 1 << 2,
            ScriptableObjects = 1 << 3,
            Scripts = 1 << 4,
            Materials = 1 << 5,
            Shaders = 1 << 6,
            Textures = 1 << 7,
            Sprites = 1 << 8,
            AnimationClips = 1 << 9,
            AudioClips = 1 << 10,
            OtherAssets = 1 << 11
        }

        internal static string GetPinnedPrefsKey() => GetKey(PinnedPrefsKeySuffix);

        public static bool RecordHierarchy
        {
            get => EditorPrefs.GetBool(GetKey(RecordHierarchyKey), true);
            set => EditorPrefs.SetBool(GetKey(RecordHierarchyKey), value);
        }

        public static bool RecordProject
        {
            get => EditorPrefs.GetBool(GetKey(RecordProjectKey), true);
            set => EditorPrefs.SetBool(GetKey(RecordProjectKey), value);
        }

        public static int MaxHistorySize
        {
            get => EditorPrefs.GetInt(GetKey(MaxHistorySizeKey), 50);
            set => EditorPrefs.SetInt(GetKey(MaxHistorySizeKey), value);
        }

        public static float RowHeight
        {
            get => EditorPrefs.GetFloat(GetKey(RowHeightKey), 20f);
            set => EditorPrefs.SetFloat(GetKey(RowHeightKey), Mathf.Clamp(value, 16f, 32f));
        }

        public static bool KeepPinnedOnClear
        {
            get => EditorPrefs.GetBool(GetKey(KeepPinnedOnClearKey), true);
            set => EditorPrefs.SetBool(GetKey(KeepPinnedOnClearKey), value);
        }

        public static bool PinnedOnTop
        {
            get => EditorPrefs.GetBool(GetKey(PinnedOnTopKey), true);
            set => EditorPrefs.SetBool(GetKey(PinnedOnTopKey), value);
        }

        public static SortMode Sorting
        {
            get => (SortMode)EditorPrefs.GetInt(GetKey(SortingKey), (int)SortMode.History);
            set => EditorPrefs.SetInt(GetKey(SortingKey), (int)value);
        }

        public static bool StartRecordingOnOpen
        {
            get => EditorPrefs.GetBool(GetKey(StartRecordingOnOpenKey), true);
            set => EditorPrefs.SetBool(GetKey(StartRecordingOnOpenKey), value);
        }

        public static TypeFilterMask TypeFilter
        {
            get
            {
                int def = (int)(
                    TypeFilterMask.SceneObjects |
                    TypeFilterMask.Prefabs |
                    TypeFilterMask.Scenes |
                    TypeFilterMask.ScriptableObjects |
                    TypeFilterMask.Scripts |
                    TypeFilterMask.Materials |
                    TypeFilterMask.Shaders |
                    TypeFilterMask.Textures |
                    TypeFilterMask.Sprites |
                    TypeFilterMask.AnimationClips |
                    TypeFilterMask.AudioClips |
                    TypeFilterMask.OtherAssets
                );

                return (TypeFilterMask)EditorPrefs.GetInt(GetKey(TypeFilterMaskKey), def);
            }
            set => EditorPrefs.SetInt(GetKey(TypeFilterMaskKey), (int)value);
        }


        public static bool ShowPinButton
        {
            get => EditorPrefs.GetBool(GetKey(ShowPinButtonKey), true);
            set => EditorPrefs.SetBool(GetKey(ShowPinButtonKey), value);
        }

        public static bool ShowInspectorButton
        {
            get => EditorPrefs.GetBool(GetKey(ShowInspectorButtonKey), true);
            set => EditorPrefs.SetBool(GetKey(ShowInspectorButtonKey), value);
        }

        public static bool ShowDeleteButton
        {
            get => EditorPrefs.GetBool(GetKey(ShowDeleteButtonKey), false);
            set => EditorPrefs.SetBool(GetKey(ShowDeleteButtonKey), value);
        }

        public static bool ShowRevealButton
        {
            get => EditorPrefs.GetBool(GetKey(ShowRevealButtonKey), false);
            set => EditorPrefs.SetBool(GetKey(ShowRevealButtonKey), value);
        }

        public static bool DontRemovePinnedOnLimit
        {
            get => EditorPrefs.GetBool(GetKey(DontRemovePinnedOnLimitKey), true);
            set => EditorPrefs.SetBool(GetKey(DontRemovePinnedOnLimitKey), value);
        }

        public static HistoryLimitBehavior HistoryLimitAction
        {
            get => (HistoryLimitBehavior)EditorPrefs.GetInt(GetKey(HistoryLimitActionKey), (int)HistoryLimitBehavior.RemoveOldest);
            set => EditorPrefs.SetInt(GetKey(HistoryLimitActionKey), (int)value);
        }

        internal static string GetKey(string key) => $"{PlayerSettings.productName}.{key}";
    }
}
