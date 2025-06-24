using Keetzap.EditorTools;
using System;
using UnityEditor;
using UnityEngine;

namespace Keetzap.EditorTools
{
    public class CollidersArrangement : BaseEditorWindow
    {
        private bool _deleteColliders = true;
        private bool _roundColliders = true;

        [MenuItem("Keetzap/Colliders utilities")]
        static void Init()
        {
            CollidersArrangement window = (CollidersArrangement)EditorWindow.GetWindow(typeof(CollidersArrangement));
            ShowWindow(window, "Collider utilites", 5, 5, 0, 5, 300, 125, false);
        }

        protected sealed override void MainSection()
        {
            GUIContent content = new();

            EditorGUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            {
                SetLabelWidth(100);
                _deleteColliders = EditorGUILayout.Toggle(new GUIContent("Delete colliders"), _deleteColliders);
                _roundColliders = EditorGUILayout.Toggle(new GUIContent("Round colliders"), _roundColliders);
                ResetLabelWidth();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(5);
            if(GUILayout.Button("Round colliders", GUILayout.ExpandWidth(true), GUILayout.Height(ButtonHeight)))
            {
                RoundColliders();

                if (_roundColliders)
                {
                    RoundColliders();
                }
            }

            content = new GUIContent("Unify colliders from children", "Unifica todos los colliders del mismo tipo de todos los hijos del objeto seleccionado y lo aplica al mismo objeto.\n\nIf the 'Delete Colliders' option is enabled, the colliders will also be removed from the children.");
            if (GUILayout.Button(content, GUILayout.ExpandWidth(true), GUILayout.Height(ButtonHeight)))
            {
                UnifyAllSelectedColliders();
            }

            content = new GUIContent("Copy colliders to selection", "Copies all colliders from the children of the selected object and adds them to the selected object itself.\n\nIf the 'Delete Colliders' option is enabled, the colliders will also be removed from the children.");

            if (GUILayout.Button(content, GUILayout.ExpandWidth(true), GUILayout.Height(ButtonHeight)))
            {
                CopyColliders();

                if (_roundColliders)
                {
                    RoundColliders();
                }
            }
        }

        private void UnifyAllSelectedColliders()
        {
            Transform[] selection = Selection.transforms;
            if(GetNoSelectionException(selection))
            {
                return;
            }

            foreach(var c in selection)
            {
                CreateCollider(c);
            }
        }

        private void CreateCollider(Transform prop)
        {
            Renderer[] renderers = prop.GetComponentsInChildren<Renderer>();

            if(renderers.Length > 0)
            {
                Bounds bounds = renderers[0].bounds;

                for(int i = 1; i < renderers.Length; i++)
                {
                    if(renderers[i].enabled)
                    {
                        bounds.Encapsulate(renderers[i].bounds);
                    }
                }

                CreateColliderFromBounds(prop, bounds);
            }
        }

        private void RoundColliders()
        {
            Transform[] selection = Selection.transforms;
            if(GetSelectionException(selection))
            {
                return;
            }

            Transform prop = selection[0];

            Collider[] colliders = prop.GetComponentsInChildren<Collider>();

            foreach(var c in colliders)
            {
                switch(c)
                {
                    case BoxCollider box:
                        RoundBoxColliderProperties(box);
                        break;
                    case CapsuleCollider capsule:
                        RoundCapsuleColliderProperties(capsule);
                        break;
                    case SphereCollider sphere:
                        RoundSphereColliderProperties(sphere);
                        break;
                }
            }
        }

        static private void RoundBoxColliderProperties(BoxCollider c)
        {
            c.center = RoundCollider(c.center);
            c.size = RoundCollider(c.size);
        }

        static private void RoundCapsuleColliderProperties(CapsuleCollider c)
        {
            c.center = RoundCollider(c.center);
            c.radius = RoundCollider(c.radius);
            c.height = RoundCollider(c.height);
        }

        static private void RoundSphereColliderProperties(SphereCollider c)
        {
            c.center = RoundCollider(c.center);
            c.radius = RoundCollider(c.radius);
        }

        static private float RoundCollider(float value)
        {
            return (float)Math.Round(value, 2);
        }

        static private Vector3 RoundCollider(Vector3 value)
        {
            return new Vector3((float)Math.Round(value.x, 2), (float)Math.Round(value.y, 2), (float)Math.Round(value.z, 2));
        }

        private void CreateColliderFromBounds(Transform prop, Bounds bounds)
        {
            Collider[] colliders = prop.GetComponentsInChildren<Collider>();
            BoxCollider collider = prop.gameObject.AddComponent<BoxCollider>();

            Vector3 center = bounds.center - prop.transform.position;
            center = new Vector3((float)Math.Round(center.x, 2), (float)Math.Round(center.y, 2), (float)Math.Round(center.z, 2));
            Vector3 size = new Vector3((float)Math.Round(bounds.size.x, 2), (float)Math.Round(bounds.size.y, 2), (float)Math.Round(bounds.size.z, 2));

            collider.size = size;
            collider.center = center;

            DeleteColliders(colliders);
        }

        private void CopyColliders()
        {
            Transform[] selection = Selection.transforms;
            if(GetSelectionException(selection))
            {
                return;
            }

            Transform prop = selection[0];

            Collider[] colliders = prop.GetComponentsInChildren<Collider>();

            foreach(var c in colliders)
            {
                c.isTrigger = true;

                switch(c)
                {
                    case BoxCollider box:
                        CopyBoxColliderProperties(box, prop);
                        break;
                    case CapsuleCollider capsule:
                        CopyCapsuleColliderProperties(capsule, prop);
                        break;
                    case SphereCollider sphere:
                        CopySphereColliderProperties(sphere, prop);
                        break;
                }

                DestroyImmediate(c);
            }

            DeleteColliders(colliders);
        }

        static private void CopyBoxColliderProperties(BoxCollider c, Transform prop)
        {
            BoxCollider collider = prop.gameObject.AddComponent<BoxCollider>();
            collider.center = c.center + c.gameObject.transform.position - prop.localPosition;
            collider.size = c.size;
        }

        static private void CopyCapsuleColliderProperties(CapsuleCollider c, Transform prop)
        {
            CapsuleCollider collider = prop.gameObject.AddComponent<CapsuleCollider>();
            collider.center = c.center + c.gameObject.transform.position - prop.localPosition;
            collider.radius = c.radius;
            collider.height = c.height;
        }

        static private void CopySphereColliderProperties(SphereCollider c, Transform prop)
        {
            SphereCollider collider = prop.gameObject.AddComponent<SphereCollider>();
            collider.center = c.center + c.gameObject.transform.position - prop.localPosition;
            collider.radius = c.radius;
        }

        private void DeleteColliders(Collider[] colliders)
        {
            if(_deleteColliders)
            {
                foreach(var c in colliders)
                {
                    DestroyImmediate(c);
                }
            }
        }

        private bool GetNoSelectionException(Transform[] selection)
        {
            bool errorSel = false;

            if(selection.Length == 0)
            {
                EditorUtility.DisplayDialog("Create Prefabs", "Nothing selected!\nPlease, select the root of a Zone prefab.", "OK");
                errorSel = true;
            }

            return errorSel;
        }

        private bool GetSelectionException(Transform[] selection)
        {
            bool errorSel = GetNoSelectionException(selection);

            if(selection.Length > 1)
            {
                EditorUtility.DisplayDialog("Create Prefabs", "More than one object selected\nPlease, select ONLY a single object.", "OK");
                errorSel = true;
            }

            return errorSel;
        }
    }
}
