//-----------------------------------------------------------------------
// SelectionHistoryPlusButtonPopup.cs
//
// Copyright 2026 Social Point SL. All rights reserved.
//
//-----------------------------------------------------------------------
using UnityEditor;
using UnityEngine;

namespace Keetzap.SelectionHistoryPlus.Editor
{
    internal sealed class SelectionHistoryPlusButtonPopup : PopupWindowContent
    {
        private readonly SelectionHistoryPlus _owner;

        public SelectionHistoryPlusButtonPopup(SelectionHistoryPlus owner) => _owner = owner;

        public override Vector2 GetWindowSize() => new(230f, 130f);

        public override void OnGUI(Rect rect)
        {
            EditorGUILayout.LabelField("Button Configuration", EditorStyles.boldLabel);
            EditorGUILayout.Space(4);
            EditorGUI.BeginChangeCheck();
            EditorGUI.indentLevel++;
            {
                SelectionHistoryPlusPrefs.ShowPinButton = EditorGUILayout.ToggleLeft($" {SelectionHistoryPlusLabels.Pin}/{SelectionHistoryPlusLabels.Unpin}", SelectionHistoryPlusPrefs.ShowPinButton);
                SelectionHistoryPlusPrefs.ShowInspectorButton = EditorGUILayout.ToggleLeft($" {SelectionHistoryPlusLabels.ShowInInspector}", SelectionHistoryPlusPrefs.ShowInspectorButton);
                SelectionHistoryPlusPrefs.ShowDeleteButton = EditorGUILayout.ToggleLeft($" {SelectionHistoryPlusLabels.DeleteFromHistory}", SelectionHistoryPlusPrefs.ShowDeleteButton);
                SelectionHistoryPlusPrefs.ShowRevealButton = EditorGUILayout.ToggleLeft($" {SelectionHistoryPlus.GetRevealInFileBrowserLabel()}", SelectionHistoryPlusPrefs.ShowRevealButton);
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();

            if (EditorGUI.EndChangeCheck())
            {
                _owner.Repaint();
            }
        }
    }
}
