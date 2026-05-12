using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Keetzap.EditorTools
{
    public class ObjectReplacer : BaseEditorWindow
    {
        private GameObject _replacementObject;
        private  bool _enableOffset;
        private  float _offMinValX = -0.1f;
        private  float _offMinValY = -0.1f;
        private  float _offMinValZ = -0.1f;
        private  float _offMaxValX = 0.1f;
        private  float _offMaxValY = 0.1f;
        private  float _offMaxValZ = 0.1f;
        private  bool _offSymmetryX = false;
        private  bool _offSymmetryY = false;
        private  bool _offSymmetryZ = false;
        private  bool _unlockOffsetValues;
        private  float _offsetLimit = 1;
        private  bool _matchRotation = true;
        private  bool _enableRotation;
        private  bool _applyRotationOnYAxis = true;
        private  Vector2 _rotation = new Vector2(0, 360);
        private  bool _matchScale = true;
        private  bool _enableScale;
        private  float _slcMinValX = -0.1f;
        private  float _slcMinValY = -0.1f;
        private  float _slcMinValZ = -0.1f;
        private  float _slcMaxValX = 0.1f;
        private  float _slcMaxValY = 0.1f;
        private  float _slcMaxValZ = 0.1f;
        private  bool _slcSymmetryX = false;
        private  bool _slcSymmetryY = false;
        private  bool _slcSymmetryZ = false;
        private  bool _unlockScaleValues;
        private float _scaleLimit = 1;
        private bool _deleteSourceObjects = true;
        private bool _hideSourceObjects = false;
        private bool _unparentObjects = false;
        private bool _hideNewObjects = false;
        private bool _selectNewObjects = false;

        [MenuItem("Keetzap/Object Replacer")]
        static void Init()
        {
            ObjectReplacer window = (ObjectReplacer)EditorWindow.GetWindow(typeof(ObjectReplacer));
            ShowWindow(window, "Object replacer", 10, 5, 0, 5, 300, 400, 684, true);
        }

        protected sealed override void MainSection()
        {
            _replacementObject = (GameObject)EditorGUILayout.ObjectField("Replacement Prefab", _replacementObject, typeof(GameObject), false);
            Decorators.Separator();
            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("Offset parameters", EditorStyles.boldLabel);
            Decorators.SeparatorSimple();
            _enableOffset = EditorGUILayout.Toggle(new GUIContent("Add random offset"), _enableOffset);
            EditorGUI.BeginDisabledGroup(!_enableOffset);
            {
                _offsetLimit = EditorGUILayout.FloatField(new GUIContent("    Offset Limit"), _offsetLimit);
                _offsetLimit = Mathf.Clamp(_offsetLimit, 0.01f, Mathf.Infinity);
                
                ShowSliders(ref _offMinValX, ref _offMaxValX, ref _offSymmetryX, ref _offMinValY, ref _offMaxValY, ref _offSymmetryY, ref _offMinValZ, ref _offMaxValZ, ref _offSymmetryZ, _offsetLimit, ref _unlockOffsetValues);
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("Rotation parameters", EditorStyles.boldLabel);
            Decorators.SeparatorSimple();
            _matchRotation = EditorGUILayout.Toggle(new GUIContent("Match source rotation"), _matchRotation);
            _enableRotation = EditorGUILayout.Toggle(new GUIContent("Add random rotation"), _enableRotation);
            EditorGUI.BeginDisabledGroup(!_enableRotation);
            {
                _applyRotationOnYAxis = EditorGUILayout.Toggle(new GUIContent("    Apply only on Y axis"), _applyRotationOnYAxis);
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Label("    Rotation", GUILayout.MaxWidth(80));
                    _rotation.x = EditorGUILayout.FloatField(_rotation.x, GUILayout.MaxWidth(60));
                    EditorGUILayout.MinMaxSlider(ref _rotation.x, ref _rotation.y, 0, 360);
                    _rotation.y = EditorGUILayout.FloatField(_rotation.y, GUILayout.MaxWidth(60));
                }
                EditorGUILayout.EndHorizontal(); 
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("Scale parameters", EditorStyles.boldLabel);
            Decorators.SeparatorSimple();
            _matchScale = EditorGUILayout.Toggle(new GUIContent("Match source scale"), _matchScale);
            _enableScale = EditorGUILayout.Toggle(new GUIContent("Add random scale"), _enableScale);
            EditorGUI.BeginDisabledGroup(!_enableScale);
            {
                _scaleLimit = EditorGUILayout.FloatField(new GUIContent("    Scale Limit"), _scaleLimit);
                _scaleLimit = Mathf.Clamp(_scaleLimit, 0.01f, Mathf.Infinity);
                ShowSliders(ref _slcMinValX, ref _slcMaxValX, ref _slcSymmetryX, ref _slcMinValY, ref _slcMaxValY, ref _slcSymmetryY, ref _slcMinValZ, ref _slcMaxValZ, ref _slcSymmetryZ, _scaleLimit, ref _unlockScaleValues);
            }
            EditorGUI.EndDisabledGroup();
            Decorators.Separator();
            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("Other parameters", EditorStyles.boldLabel);
            Decorators.SeparatorSimple();
            SetLabelWidth(180);
            _deleteSourceObjects = EditorGUILayout.Toggle(new GUIContent("    Delete Selected Objects"), _deleteSourceObjects);
            EditorGUI.BeginDisabledGroup(_deleteSourceObjects);
            {
                _hideSourceObjects = EditorGUILayout.Toggle(new GUIContent("    Hide Selected Objects"), _hideSourceObjects);
                _unparentObjects = EditorGUILayout.Toggle(new GUIContent("    Unparent Selected Objects"), _unparentObjects);
            }
            EditorGUI.EndDisabledGroup();
            _hideNewObjects = EditorGUILayout.Toggle(new GUIContent("    Hide New Objects"), _hideNewObjects);
            EditorGUI.BeginDisabledGroup(_hideNewObjects);
            {
                _selectNewObjects = EditorGUILayout.Toggle(new GUIContent("    Select All New Objects"), _selectNewObjects);
            }
            EditorGUI.EndDisabledGroup();
            ResetLabelWidth();
            Decorators.Separator();
            ShowButtons();
        }

        protected sealed override void DrawFooter(string helpMessage)
        {
            base.DrawFooter(
                "This tool replaces a list of selected objects from the scene by the given prefab. Also, it provides other functionalities such us\n\n" +
                "\t· Offset, Rotation and Scale randomness\n" +
                "\t· Delete, hide or unparent the source objects\n" +
                "\t· Hide the new objects\n"
                );
        }

        private void ShowSliders(ref float minValX, ref float maxValX, ref bool symmetryX, ref float minValY, ref float maxValY, ref bool symmetryY, ref float minValZ, ref float maxValZ, ref bool symmetryZ, float limit, ref bool lockValues)
        {
            EditorGUILayout.Space(3);
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.BeginVertical();
                {
                    MinMaxSliderValues("    Offset X", ref minValX, ref maxValX, ref symmetryX, limit);
                    EditorGUI.BeginDisabledGroup(!lockValues);
                    {
                        MinMaxSliderValues("    Offset Y", ref minValY, ref maxValY, ref symmetryY, limit);
                        MinMaxSliderValues("    Offset Z", ref minValZ, ref maxValZ, ref symmetryZ, limit);
                    }
                    EditorGUI.EndDisabledGroup();

                }
                EditorGUILayout.EndVertical();

                Texture2D iconTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Keetzap/Scripts/Editor/Icons/ICO_GUI_PadlockClosed.png");
                lockValues = GUILayout.Toggle(lockValues, new GUIContent(iconTexture), "Button", GUILayout.MaxHeight(64), GUILayout.MaxWidth(28));

                if(!lockValues)
                {
                    minValY = minValZ = minValX;
                    maxValY = maxValZ = maxValX;
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void MinMaxSliderValues(string label, ref float minVal, ref float maxVal, ref bool symmetry, float limit)
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label(label, GUILayout.MaxWidth(80));
                minVal = EditorGUILayout.FloatField(minVal, GUILayout.MaxWidth(60));
                EditorGUILayout.MinMaxSlider(ref minVal, ref maxVal, -limit, limit);
                maxVal = EditorGUILayout.FloatField(maxVal, GUILayout.MaxWidth(60));
                Texture2D iconTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Keetzap/Scripts/Editor/Icons/ICO_GUI_Symmetry.png");
                symmetry = GUILayout.Toggle(symmetry, new GUIContent(iconTexture), "Button", GUILayout.MaxHeight(20), GUILayout.MaxWidth(24));

                if(!symmetry)
                {
                    if(maxVal < 0)
                    {
                        maxVal = -maxVal;
                    }
                    else
                    {
                        minVal = -maxVal;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        void ShowButtons()
        {
            EditorGUILayout.Space(3);
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Replace Objects", GUILayout.Height(28)))
                {
                    ReplaceObjects();
                }
                if (GUILayout.Button("Apply Transformations Only", GUILayout.Height(28)))
                {
                    ApplyTransformationsOnly();

                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void ReplaceObjects()
        {
            var selection = Selection.gameObjects;

            Undo.IncrementCurrentGroup();
            int undoGroup = Undo.GetCurrentGroup();

            var newSelection = new List<GameObject>();

            foreach (GameObject o in selection)
            {
                var go = (GameObject)PrefabUtility.InstantiatePrefab(_replacementObject);

                Undo.RegisterCreatedObjectUndo(go, "Replace Object");

                go.transform.parent = o.transform.parent;
                go.name = _replacementObject.name;
                go.transform.localPosition = o.transform.localPosition;

                if(_matchRotation)
                {
                    go.transform.rotation = o.transform.rotation;
                }

                if(_matchScale)
                {
                    go.transform.localScale = o.transform.localScale;
                }

                ApplyRandomTransformations(go);

                if (_deleteSourceObjects)
                {
                    Undo.DestroyObjectImmediate(o);
                }
                else
                {
                    if (_unparentObjects)
                    {
                        Undo.RecordObject(go.transform, "Unparent Selected Object");
                        o.transform.parent = null;
                    }

                    if (_hideSourceObjects)
                    {
                        Undo.RecordObject(o, "Hide Selected Object");
                        o.SetActive(false);
                    }
                }

                if (_hideNewObjects)
                {
                    Undo.RecordObject(go.transform, "Hide New Objects");
                    go.SetActive(false);
                }
                else
                {
                    newSelection.Add(go);
                }
            }

            if (!_hideNewObjects && _selectNewObjects)
            {
                Selection.objects = newSelection.ToArray();
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Undo.CollapseUndoOperations(undoGroup);
        }

        private void ApplyTransformationsOnly()
        {
            var selection = Selection.gameObjects;

            foreach (GameObject o in selection)
            {
                ApplyRandomTransformations(o);
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        private void ApplyRandomTransformations(GameObject newObject)
        {
            if(_enableOffset)
            {
                ApplyRandomOffset(newObject);
            }

            if(_enableRotation)
            {
                ApplyRandomRotation(newObject);
            }

            if(_enableScale)
            {
                ApplyRandomScale(newObject);
            }
        }

        private void ApplyRandomOffset(GameObject newObject)
        {
            var offsetX = Random.Range(_offMinValX, _offMaxValX);
            var offsetY = 0f;
            var offsetZ = 0f;

            if(_unlockOffsetValues)
            {
                offsetY = Random.Range(_offMinValY, _offMaxValY);
                offsetZ = Random.Range(_offMinValZ, _offMaxValZ);
            }
            else
            {
                offsetY = offsetZ = offsetX;
            }

            newObject.transform.position += new Vector3(offsetX, offsetY, offsetZ);
        }

        private void ApplyRandomRotation(GameObject newObject)
        {
            var rotationX = 0f;
            var rotationY = Random.Range(_rotation.x, _rotation.y);
            var rotationZ = 0f;

            if(!_applyRotationOnYAxis)
            {
                rotationX = Random.Range(_rotation.x, _rotation.y);
                rotationZ = Random.Range(_rotation.x, _rotation.y);
            }

            newObject.transform.rotation *= Quaternion.Euler(rotationX, rotationY, rotationZ);
        }

        private void ApplyRandomScale(GameObject newObject)
        {
            var scaleX = Random.Range(_slcMinValX, _slcMaxValX);
            var scaleY = 0f;
            var scaleZ = 0f;

            if(_unlockScaleValues)
            {
                scaleY = Random.Range(_slcMinValY, _slcMaxValY);
                scaleZ = Random.Range(_slcMinValZ, _slcMaxValZ);
            }
            else
            {
                scaleY = scaleZ = scaleX;
            }

            newObject.transform.localScale += new Vector3(scaleX, scaleY, scaleZ);
        }
    }
}
