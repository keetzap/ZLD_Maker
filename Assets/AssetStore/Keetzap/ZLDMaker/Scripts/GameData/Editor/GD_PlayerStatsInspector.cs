using UnityEngine;
using UnityEditor;

namespace Keetzap.ZeldaMaker
{
    [CustomEditor(typeof(GD_PlayerStats))]
    public class GD_PlayerStatsInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GUILayout.Space(15);
            EditorGUILayout.LabelField("Preset Actions", EditorStyles.boldLabel);
            
            if (GUILayout.Button("Apply Default Preset", GUILayout.Height(30)))
            {
                GD_PlayerStats stats = (GD_PlayerStats)target;
                Undo.RecordObject(stats, "Apply Preset");
                
                stats.ApplyPreset();
                
                EditorUtility.SetDirty(stats);
            }
        }
    }
}
