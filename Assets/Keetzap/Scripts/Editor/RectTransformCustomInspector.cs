using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Keetzap.EditorTools
{
    [CustomEditor(typeof(RectTransform))]
    public class RectTransformCustomInspector : UnityEditor.Editor
    {
        private UnityEditor.Editor _editorInstance;
        private Transform _transform;

        private static readonly float[] _pivotSnapValues = { 0f, 0.5f, 1f };

        private const string MenuPath = "Keetzap/Show RectTransform Utilities";
        private const string PrefKey = "Keetzap_ShowRectTransformUtilities";

        private static bool _showUtilities;

        [InitializeOnLoadMethod]
        private static void InitMenu()
        {
            EditorApplication.delayCall += () =>
            {
                _showUtilities = EditorPrefs.GetBool(PrefKey, false);
                Menu.SetChecked(MenuPath, _showUtilities);
            };
        }

        [MenuItem(MenuPath, false, 200)]
        private static void ToggleShowUtilities()
        {
            _showUtilities = !_showUtilities;
            EditorPrefs.SetBool(PrefKey, _showUtilities);
            Menu.SetChecked(MenuPath, _showUtilities);

            foreach (var editor in ActiveEditorTracker.sharedTracker.activeEditors)
            {
                if (editor.target is RectTransform)
                    editor.Repaint();
            }
        }

        void OnEnable()
        {
            _editorInstance = CreateEditor(targets, Type.GetType("UnityEditor.RectTransformEditor, UnityEditor"));
            _transform = target as Transform;
        }

        void OnDisable()
        {
            if(_editorInstance == null)
            {
                return;
            }

            var disableMethod = _editorInstance.GetType().GetMethod("OnDisable", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if(disableMethod != null)
            {
                disableMethod.Invoke(_editorInstance, null);
            }

            DestroyImmediate(_editorInstance);
        }

        public override void OnInspectorGUI()
        {
            _editorInstance.OnInspectorGUI();

            if(_showUtilities)
            {
                RectTransform rt = (RectTransform)target;

                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                EditorGUILayout.BeginHorizontal();

                if(GUILayout.Button("Round Values", GUILayout.MinHeight(24)))
                {
                    RoundNumbers(rt);
                }

                if(GUILayout.Button("Corners to Anchors", GUILayout.MinHeight(24)))
                {
                    CornersToAnchors(rt);
                }

                if(GUILayout.Button("Anchors to Corners", GUILayout.MinHeight(24)))
                {
                    AnchorsToCorners(rt);
                }

                if(GUILayout.Button("Anchors to Center", GUILayout.MinHeight(24)))
                {
                    AnchorsToCenter(rt);
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        private void RoundNumbers(RectTransform rt)
        {
            Undo.RecordObject(rt, "Round RectTransform Values");

            rt.pivot = SnapVector2(rt.pivot);

            var stretchX = rt.anchorMin.x != rt.anchorMax.x;
            var stretchY = rt.anchorMin.y != rt.anchorMax.y;

            if(stretchX || stretchY)
            {
                Vector2 offsetMin = rt.offsetMin;
                Vector2 offsetMax = rt.offsetMax;

                if(stretchX)
                {
                    offsetMin.x = RoundFloat(offsetMin.x);
                    offsetMax.x = RoundFloat(offsetMax.x);
                }

                if(stretchY)
                {
                    offsetMin.y = RoundFloat(offsetMin.y);
                    offsetMax.y = RoundFloat(offsetMax.y);
                }

                rt.offsetMin = offsetMin;
                rt.offsetMax = offsetMax;

                if(!stretchX || !stretchY)
                {
                    rt.anchoredPosition = RoundVector2(rt.anchoredPosition);
                    rt.sizeDelta = RoundVector2(rt.sizeDelta);
                }
            }
            else
            {
                rt.anchoredPosition = RoundVector2(rt.anchoredPosition);
                rt.sizeDelta = RoundVector2(rt.sizeDelta);
            }

            EditorUtility.SetDirty(rt);
        }

        private Vector2 RoundVector2(Vector2 v)
        {
            return new Vector2(RoundFloat(v.x), RoundFloat(v.y));
        }

        private Vector2 SnapVector2(Vector2 v)
        {
            return new Vector2(SnapFloat(v.x), SnapFloat(v.y));
        }

        private float RoundFloat(float value)
        {
            return Mathf.Round(value);
        }

        private float SnapFloat(float value)
        {
            foreach(var allowed in _pivotSnapValues)
            {
                if(Mathf.Abs(value - allowed) < 0.001f)
                {
                    return allowed;
                }
            }
            return value;
        }

        private void CornersToAnchors(RectTransform rt)
        {
            Undo.RecordObject(rt, "CornersToAnchors RectTransform Values");

            rt.offsetMin = rt.offsetMax = new Vector2(0, 0);

            EditorUtility.SetDirty(rt);
        }

        private void AnchorsToCorners(RectTransform rt)
        {
            RectTransform pt = rt.parent as RectTransform;

            if(pt == null)
            {
                return;
            }

            Undo.RecordObject(rt, "AnchorsToCorners RectTransform Values");

            Vector2 newAnchorsMin = new Vector2(rt.anchorMin.x + rt.offsetMin.x / pt.rect.width,
                                                rt.anchorMin.y + rt.offsetMin.y / pt.rect.height);
            Vector2 newAnchorsMax = new Vector2(rt.anchorMax.x + rt.offsetMax.x / pt.rect.width,
                                                rt.anchorMax.y + rt.offsetMax.y / pt.rect.height);
            rt.anchorMin = newAnchorsMin;
            rt.anchorMax = newAnchorsMax;
            rt.offsetMin = rt.offsetMax = new Vector2(0, 0);

            EditorUtility.SetDirty(rt);
        }

        private void AnchorsToCenter(RectTransform rt)
        {
            RectTransform pt = rt.parent as RectTransform;

            if(pt == null)
            {
                return;
            }

            Undo.RecordObject(rt, "AnchorsToCenter RectTransform Values");

            Vector2 newAnchors = Rect.PointToNormalized(pt.rect, rt.localPosition);
            Vector2 newOffsetMin = rt.rect.min;
            Vector2 newOffsetMax = rt.rect.max;

            rt.anchorMin = rt.anchorMax = newAnchors;
            rt.offsetMin = newOffsetMin;
            rt.offsetMax = newOffsetMax;

            EditorUtility.SetDirty(rt);
        }
    }

}
