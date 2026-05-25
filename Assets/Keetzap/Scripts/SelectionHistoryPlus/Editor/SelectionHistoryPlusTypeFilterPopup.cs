//-----------------------------------------------------------------------
// SelectionHistoryPlusTypeFilterPopup.cs
//
// Copyright 2026 Social Point SL. All rights reserved.
//
//-----------------------------------------------------------------------
using UnityEditor;
using UnityEngine;

namespace Keetzap.SelectionHistoryPlus.Editor
{
    internal sealed class SelectionHistoryPlusTypeFilterPopup : PopupWindowContent
    {
        private readonly SelectionHistoryPlus _owner;
        private Vector2 _scroll;

        public SelectionHistoryPlusTypeFilterPopup(SelectionHistoryPlus owner) => _owner = owner;

        public override Vector2 GetWindowSize() => new(260f, 380f);

        public override void OnGUI(Rect rect)
        {
            EditorGUILayout.LabelField("Filter by Type", EditorStyles.boldLabel);
            EditorGUILayout.Space(4);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("All", EditorStyles.miniButtonLeft))
                {
                    SelectionHistoryPlusPrefs.TypeFilter = (SelectionHistoryPlusPrefs.TypeFilterMask)(-1);
                }

                if (GUILayout.Button("None", EditorStyles.miniButtonRight))
                {
                    SelectionHistoryPlusPrefs.TypeFilter = 0;
                }
            }

            EditorGUILayout.Space(6);

            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            {
                DrawToggle(SelectionHistoryPlusPrefs.TypeFilterMask.SceneObjects, "Scene Objects");
                DrawToggle(SelectionHistoryPlusPrefs.TypeFilterMask.Prefabs, "Prefabs");
                DrawToggle(SelectionHistoryPlusPrefs.TypeFilterMask.Scenes, "Scenes");
                DrawToggle(SelectionHistoryPlusPrefs.TypeFilterMask.Scripts, "Scripts");
                DrawToggle(SelectionHistoryPlusPrefs.TypeFilterMask.ScriptableObjects, "ScriptableObjects");
                DrawToggle(SelectionHistoryPlusPrefs.TypeFilterMask.Materials, "Materials");
                DrawToggle(SelectionHistoryPlusPrefs.TypeFilterMask.Shaders, "Shaders");
                DrawToggle(SelectionHistoryPlusPrefs.TypeFilterMask.Textures, "Textures");
                DrawToggle(SelectionHistoryPlusPrefs.TypeFilterMask.Sprites, "Sprites");
                DrawToggle(SelectionHistoryPlusPrefs.TypeFilterMask.AnimationClips, "AnimationClips");
                DrawToggle(SelectionHistoryPlusPrefs.TypeFilterMask.AudioClips, "AudioClips");
                DrawToggle(SelectionHistoryPlusPrefs.TypeFilterMask.OtherAssets, "Other Assets");
            }
            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space(6);
            EditorGUILayout.HelpBox("Unselected types won’t be recorded in history.", MessageType.Info);
        }

        private void DrawToggle(SelectionHistoryPlusPrefs.TypeFilterMask flag, string label)
        {
            var mask = SelectionHistoryPlusPrefs.TypeFilter;
            bool has = mask.HasFlag(flag);
            bool next = EditorGUILayout.ToggleLeft(label, has);

            if (next != has)
            {
                if (next)
                {
                    mask |= flag;
                }
                else
                {
                    mask &= ~flag;
                }

                SelectionHistoryPlusPrefs.TypeFilter = mask;
                _owner.Repaint();
            }
        }
    }
}
