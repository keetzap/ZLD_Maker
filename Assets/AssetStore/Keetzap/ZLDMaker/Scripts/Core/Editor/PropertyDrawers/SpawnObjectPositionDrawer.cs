using Keetzap.Core;
using UnityEditor;
using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    [CustomPropertyDrawer(typeof(SpawnObjectPosition))]
    public class SpawnObjectPositionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            _ = EditorGUI.BeginProperty(rect, label, property);
            {
                EditorGUILayout.Space(-20);
                SerializedProperty anchor = property.FindPropertyRelative("anchorType");
                EditorGUILayout.PropertyField(anchor, new GUIContent("Anchor type"));

                if (anchor.enumValueIndex == 0)
                {
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("anchorTransform"), new GUIContent("Transform"));
                }
                else
                {
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("anchorOffset"), new GUIContent("Offset"));
                }
            }
            EditorGUI.EndProperty();
            
        }
    }
}
