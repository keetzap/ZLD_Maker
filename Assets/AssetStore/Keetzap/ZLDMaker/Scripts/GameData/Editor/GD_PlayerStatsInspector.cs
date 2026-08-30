using UnityEditor;
using UnityEngine;
using Keetzap.Core;

namespace Keetzap.ZeldaMaker
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(GD_PlayerStats))]
    public class GD_PlayerStatsInspector : BaseEditor
    {
        private GD_PlayerStats playerStats;

        private SerializedProperty lifesMaxCapacity;
        private SerializedProperty currentCapacity;
        private SerializedProperty currentLifes;
        private SerializedProperty initialLifes;

        private SerializedProperty gems;
        private SerializedProperty silverKeys;
        private SerializedProperty goldenKeys;
        private SerializedProperty bossKey;

        private SerializedProperty presetLifesMaxCapacity;
        private SerializedProperty presetCurrentCapacity;
        private SerializedProperty presetCurrentLifes;
        private SerializedProperty presetInitialLifes;

        private SerializedProperty presetGems;
        private SerializedProperty presetSilverKeys;
        private SerializedProperty presetGoldenKeys;
        private SerializedProperty presetBossKey;

        void OnEnable()
        {
            playerStats = (GD_PlayerStats)target;

            lifesMaxCapacity = serializedObject.FindProperty(GD_PlayerStats.Fields.LifesMaxCapacity);
            currentCapacity = serializedObject.FindProperty(GD_PlayerStats.Fields.CurrentCapacity);
            currentLifes = serializedObject.FindProperty(GD_PlayerStats.Fields.CurrentLifes);
            initialLifes = serializedObject.FindProperty(GD_PlayerStats.Fields.InitialLifes);

            gems = serializedObject.FindProperty(GD_PlayerStats.Fields.Gems);
            silverKeys = serializedObject.FindProperty(GD_PlayerStats.Fields.SilverKeys);
            goldenKeys = serializedObject.FindProperty(GD_PlayerStats.Fields.GoldenKeys);
            bossKey = serializedObject.FindProperty(GD_PlayerStats.Fields.BossKey);

            presetLifesMaxCapacity = serializedObject.FindProperty(GD_PlayerStats.Fields.PresetLifesMaxCapacity);
            presetCurrentCapacity = serializedObject.FindProperty(GD_PlayerStats.Fields.PresetCurrentCapacity);
            presetCurrentLifes = serializedObject.FindProperty(GD_PlayerStats.Fields.PresetCurrentLifes);
            presetInitialLifes = serializedObject.FindProperty(GD_PlayerStats.Fields.PresetInitialLifes);

            presetGems = serializedObject.FindProperty(GD_PlayerStats.Fields.PresetGems);
            presetSilverKeys = serializedObject.FindProperty(GD_PlayerStats.Fields.PresetSilverKeys);
            presetGoldenKeys = serializedObject.FindProperty(GD_PlayerStats.Fields.PresetGoldenKeys);
            presetBossKey = serializedObject.FindProperty(GD_PlayerStats.Fields.PresetBossKey);
        }

        public override void OnInspectorGUI()
        {
            InitInspector();

            Section("PLAYER STATS", SectionPlayerStats);
            Section("KEYS & COLLECTABLES", SectionKeysAndCollectables);
            Section("PRESET ACTIONS", SectionPresetActions);
            Section("PRESET VALUES", SectionPresetValues);

            EndInspector(playerStats, "Player Stats Asset");
        }

        private void SectionPlayerStats()
        {
            EditorGUILayout.PropertyField(lifesMaxCapacity, new GUIContent("Max Capacity (HP)"));
            EditorGUILayout.PropertyField(currentCapacity, new GUIContent("Current Capacity"));
            EditorGUILayout.PropertyField(currentLifes, new GUIContent("Current Lifes"));
            EditorGUILayout.PropertyField(initialLifes, new GUIContent("Initial Lifes"));
        }

        private void SectionKeysAndCollectables()
        {
            EditorGUILayout.PropertyField(gems, new GUIContent("Gems"));
            EditorGUILayout.PropertyField(silverKeys, new GUIContent("Silver Keys"));
            EditorGUILayout.PropertyField(goldenKeys, new GUIContent("Golden Keys"));
            EditorGUILayout.PropertyField(bossKey, new GUIContent("Boss Key"));
        }

        private void SectionPresetActions()
        {
            if (GUILayout.Button("Apply Default Preset", GUILayout.Height(30)))
            {
                Undo.RecordObject(playerStats, "Apply Preset Stats");
                playerStats.ApplyPreset();
                EditorUtility.SetDirty(playerStats);
            }
        }

        private void SectionPresetValues()
        {
            EditorGUILayout.PropertyField(presetLifesMaxCapacity, new GUIContent("Preset Max Capacity"));
            EditorGUILayout.PropertyField(presetCurrentCapacity, new GUIContent("Preset Current Capacity"));
            EditorGUILayout.PropertyField(presetCurrentLifes, new GUIContent("Preset Current Lifes"));
            EditorGUILayout.PropertyField(presetInitialLifes, new GUIContent("Preset Initial Lifes"));
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(presetGems, new GUIContent("Preset Gems"));
            EditorGUILayout.PropertyField(presetSilverKeys, new GUIContent("Preset Silver Keys"));
            EditorGUILayout.PropertyField(presetGoldenKeys, new GUIContent("Preset Golden Keys"));
            EditorGUILayout.PropertyField(presetBossKey, new GUIContent("Preset Boss Key"));
        }
    }
}
