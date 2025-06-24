using Keetzap.Core;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    [CustomEditor(typeof(RespawnSystem))]
    public class RespawnSystemInspector : BaseEditor
    {
        private RespawnSystem respawnSystem;

        private SerializedProperty currentRespawnPoint;
        private SerializedProperty respawningPoints;

        private ReorderableList list;

        protected virtual void OnEnable()
        {
            respawnSystem = (RespawnSystem)target;

            currentRespawnPoint = serializedObject.FindProperty(RespawnSystem.Fields.CurrentRespawnPoint);
            respawningPoints = serializedObject.FindProperty(RespawnSystem.Fields.RespawningPoints);

            list = new ReorderableList(serializedObject, respawningPoints, true, false, true, true);
        }

        protected void SectionRespawningPoints()
        {
            Decorators.HeaderBig("List of Spawning Points");
            EditorGUILayout.Space(3);
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(GUIContent.none, GUILayout.MaxWidth(12));
                list.drawElementCallback = DrawListItems;
                list.DoLayoutList();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(3);
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(GUIContent.none, GUILayout.MaxWidth(12));
                if (GUILayout.Button("Fill list with children", GUILayout.Height(24)))
                {
                    respawnSystem.AddRespawnPoints();
                }
                EditorGUILayout.LabelField(GUIContent.none, GUILayout.MaxWidth(3));
                if (GUILayout.Button("Clear list", GUILayout.Height(24)))
                {
                    respawnSystem.RemoveRespawnPoints();
                }
                EditorGUILayout.LabelField(GUIContent.none, GUILayout.MaxWidth(6));
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);
            EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PropertyField(currentRespawnPoint);
            EditorGUI.EndDisabledGroup();
        }

        void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);

            EditorGUI.LabelField(new Rect(rect.x + 10, rect.y, 120, EditorGUIUtility.singleLineHeight), "Respawning Point");
            EditorGUI.PropertyField(new Rect(rect.x + 130, rect.y, rect.max.x - 190, EditorGUIUtility.singleLineHeight),
                                    element.FindPropertyRelative("respawnPoint"),
                                    GUIContent.none);
        }
    }
}