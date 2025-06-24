using UnityEngine;
using UnityEditor;
using Keetzap.Core;
/*
namespace Keetzap.ZeldaMaker
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CameraTriggerRoomBase))]
    public class CameraTriggerRoomBaseInspector : BaseEditor
    {
        private SerializedProperty virtualCamera;
        private SerializedProperty blackLid;
        private SerializedProperty blackFrame;
        private SerializedProperty fadeThreshold;
        private SerializedProperty frameDistance;
        private SerializedProperty offsetFrame;
        private SerializedProperty useCustomBlackFrame;
        private SerializedProperty customBlackFrame;
        private SerializedProperty enteringFadeDuration;
        private SerializedProperty exitingFadeDuration;

        protected virtual void OnEnable()
        {
            virtualCamera = serializedObject.FindProperty(CameraTriggerRoomBase.Fields.VirtualCamera);
            blackLid = serializedObject.FindProperty(CameraTriggerRoomBase.Fields.BlackLid);
            blackFrame = serializedObject.FindProperty(CameraTriggerRoomBase.Fields.BlackFrame);
            fadeThreshold = serializedObject.FindProperty(CameraTriggerRoomBase.Fields.FadeThreshold);
            frameDistance = serializedObject.FindProperty(CameraTriggerRoomBase.Fields.FrameDistance);
            offsetFrame = serializedObject.FindProperty(CameraTriggerRoomBase.Fields.OffsetFrame);
            useCustomBlackFrame = serializedObject.FindProperty(CameraTriggerRoomBase.Fields.UseCustomBlackFrame);
            customBlackFrame = serializedObject.FindProperty(CameraTriggerRoomBase.Fields.CustomBlackFrame);
            enteringFadeDuration = serializedObject.FindProperty(CameraTriggerRoomBase.Fields.EnteringFadeDuration);
            exitingFadeDuration = serializedObject.FindProperty(CameraTriggerRoomBase.Fields.ExitingFadeDuration);
        }

        protected void SectionMainObjects()
        {
            EditorGUILayout.PropertyField(virtualCamera);
            EditorGUILayout.PropertyField(blackLid);
            EditorGUILayout.PropertyField(blackFrame);
        }

        protected void SectionBlackFrame()
        {
            Decorators.HeaderBig("Black frame properties");
            EditorGUILayout.PropertyField(fadeThreshold);
            EditorGUILayout.PropertyField(frameDistance);
            EditorGUILayout.PropertyField(offsetFrame);
            GUILayout.Space(2);
            Decorators.SeparatorSimple();
            Decorators.HeaderBig("Custom black frame");
            EditorGUIUtility.labelWidth = 200;
            EditorGUILayout.PropertyField(useCustomBlackFrame);
            EditorGUI.BeginDisabledGroup(useCustomBlackFrame.boolValue == false);
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(customBlackFrame, new GUIContent("Black frame object"));
                EditorGUI.indentLevel--;
            }
            EditorGUI.EndDisabledGroup();
        }

        protected void SectionBlackFrameBehaviour()
        {
            EditorGUIUtility.labelWidth = 270;
            EditorGUILayout.PropertyField(enteringFadeDuration, new GUIContent("Fading time when entering in a new room"));
            EditorGUILayout.PropertyField(exitingFadeDuration, new GUIContent("Fading time when exiting the current room"));
            ResetLabelWidth();
        }
    }
}*/