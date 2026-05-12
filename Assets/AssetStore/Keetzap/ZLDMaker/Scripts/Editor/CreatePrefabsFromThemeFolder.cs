using Keetzap.EditorTools;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Keetzap.ZLD_Maker.Tools
{
    public class CreatePrefabsFromThemeFolder : BaseEditorWindow
    {
        private const string MDL = "MDL_";
        private const string PRF = "PRF_";
        private const string PathColliders = "Assets/AssetStore/Keetzap/ZLDMaker/Art/Models/[Commons]/Colliders/";
        private const string Collider = "Collider_";
        private const string Models = "Assets/AssetStore/Keetzap/ZLDMaker/Art/Models/Structure/";
        private const string Prefabs = "Assets/AssetStore/Keetzap/ZLDMaker/Art/Prefabs/Structure/";
        private const string Struct = "STR_";
        private const string Extension = ".fbx";

        private readonly Dictionary<string, string> _colliders = new()
    {
        { "AngleInner_Lean_1_1_A", "AngleInner_Lean_1_1" },
        { "AngleInner_Lean_1_2_A", "AngleInner_Lean_1_2" },
        { "AngleInner_Lean_1_2_B", "AngleInner_Lean_1_2" },
        { "AngleInner_Vert_1_1_A", "AngleInner_Vert_1_1" },
        { "AngleInner_Vert_1_2_A", "AngleInner_Vert_1_2" },
        { "AngleOuter_Lean_1_1_A", "AngleOuter_Lean_1_1" },
        { "AngleOuter_Lean_1_2_A", "AngleOuter_Lean_1_2" },
        { "AngleOuter_Lean_2_2_A", "AngleOuter_Lean_2_2" },
        { "AngleOuter_Lean_2_2_B", "AngleOuter_Lean_2_2" },
        { "AngleOuter_Vert_1_1_A", "AngleOuter_Vert_1_1" },
        { "AngleOuter_Vert_1_2_A", "AngleOuter_Vert_1_2" },
        { "AngleOuter_Vert_1_2_B", "AngleOuter_Vert_1_2" },
        { "DoorMetal_Lean", "Door_Lean" },
        { "DoorMetal_Vert", "Door_Vert" },
        { "DoorStone_Lean", "Door_Lean" },
        { "DoorStone_Vert", "Door_Vert" },
        { "DoorWOFrame_Lean", "Door_Lean" },
        { "DoorWood_Lean", "Door_Lean" },
        { "DoorWood_Vert", "Door_Vert" },
        { "Floor_Brown_0_A", "Floor_1_1" },
        { "Floor_Brown_0_B", "Floor_1_1" },
        { "Floor_Brown_0_C", "Floor_1_1" },
        { "Floor_Brown_0_D", "Floor_1_1" },
        { "Floor_Brown_0_E", "Floor_1_1" },
        { "Floor_Brown_2_A", "Floor_1_1" },
        { "Floor_Brown_3_A", "Floor_1_1" },
        { "Floor_Brown_4_A", "Floor_1_1" },
        { "Floor_Brown_4_B", "Floor_1_1" },
        { "Floor_Brown_Small_1", "Floor_Small_1" },
        { "Floor_Brown_Small_2", "Floor_Small_2" },
        { "Floor_Brown_Small_3", "Floor_Small_3" },
        { "Floor_Brown_Special_A", "Floor_1_1" },
        { "Floor_Brown_Special_B", "Floor_1_1" },
        { "Floor_Dirt_0_A", "Floor_1_1" },
        { "Floor_Dirt_0_B", "Floor_1_1" },
        { "Floor_Dirt_0_C", "Floor_1_1" },
        { "Floor_Dirt_0_D", "Floor_1_1" },
        { "Floor_Dirt_0_E", "Floor_2_2" },
        { "Floor_Green_0_A", "Floor_1_1" },
        { "Floor_Green_0_B", "Floor_1_1" },
        { "Floor_Green_0_C", "Floor_1_1" },
        { "Floor_Green_0_D", "Floor_1_1" },
        { "Floor_Green_0_E", "Floor_1_1" },
        { "Floor_Green_2_A", "Floor_1_1" },
        { "Floor_Green_3_A", "Floor_1_1" },
        { "Floor_Green_4_A", "Floor_1_1" },
        { "Floor_Green_4_B", "Floor_1_1" },
        { "Floor_Green_Small_1", "Floor_Small_1" },
        { "Floor_Green_Small_2", "Floor_Small_2" },
        { "Floor_Green_Small_3", "Floor_Small_3" },
        { "Floor_Green_Special_A", "Floor_1_1" },
        { "Floor_Green_Special_B", "Floor_1_1" },
        { "Floor_Mud_1000", "Floor_1_1" },
        { "Floor_Mud_1100", "Floor_1_1" },
        { "Floor_Mud_1110", "Floor_1_1" },
        { "Floor_Mud_1111", "Floor_1_1" },
        { "Floor_Mud_2000", "Floor_1_1" },
        { "Floor_Mud_2010", "Floor_1_1" },
        { "Floor_Mud_2020", "Floor_1_1" },
        { "Floor_Mud_2100", "Floor_1_1" },
        { "Floor_Mud_2110", "Floor_1_1" },
        { "Floor_Mud_2200", "Floor_1_1" },
        { "Floor_Mud_2210", "Floor_1_1" },
        { "Floor_Mud_2220", "Floor_1_1" },
        { "Floor_Mud_2222", "Floor_1_1" },
        { "Floor_Purple_0_A", "Floor_1_1" },
        { "Floor_Purple_0_B", "Floor_1_1" },
        { "Floor_Purple_0_C", "Floor_1_1" },
        { "Floor_Purple_0_D", "Floor_1_1" },
        { "Floor_Purple_0_E", "Floor_1_1" },
        { "Floor_Purple_2_A", "Floor_1_1" },
        { "Floor_Purple_3_A", "Floor_1_1" },
        { "Floor_Purple_4_A", "Floor_1_1" },
        { "Floor_Purple_4_B", "Floor_1_1" },
        { "Floor_Purple_Small_1", "Floor_Small_1" },
        { "Floor_Purple_Small_2", "Floor_Small_2" },
        { "Floor_Purple_Small_3", "Floor_Small_3" },
        { "Stairs_Lean_B_1", "Stairs_Lean_B_1" },
        { "Stairs_Lean_B_2", "Stairs_Lean_B_2" },
        { "Stairs_Lean_C_1", "Stairs_Lean_C_1" },
        { "Stairs_Lean_C_2", "Stairs_Lean_C_2" },
        { "Stairs_Lean_L_1", "Stairs_Lean_L_1" },
        { "Stairs_Lean_L_2", "Stairs_Lean_L_2" },
        { "Stairs_Lean_R_1", "Stairs_Lean_R_1" },
        { "Stairs_Lean_R_2", "Stairs_Lean_R_2" },
        { "Stairs_Vert_B_1", "Stairs_Vert_B_1" },
        { "Stairs_Vert_B_2", "Stairs_Vert_B_2" },
        { "Stairs_Vert_C_1", "Stairs_Vert_C_1" },
        { "Stairs_Vert_C_2", "Stairs_Vert_C_2" },
        { "Stairs_Vert_L_1", "Stairs_Vert_L_1" },
        { "Stairs_Vert_L_2", "Stairs_Vert_L_2" },
        { "Stairs_Vert_R_1", "Stairs_Vert_R_1" },
        { "Stairs_Vert_R_2", "Stairs_Vert_R_2" },
        { "Wall_Lean_1_1_A", "Wall_Lean_1_1" },
        { "Wall_Lean_1_2_A", "Wall_Lean_1_2" },
        { "Wall_Lean_1_2_B", "Wall_Lean_1_2" },
        { "Wall_Lean_2_1_A", "Wall_Lean_2_1" },
        { "Wall_Lean_2_2_A", "Wall_Lean_2_2" },
        { "Wall_Lean_2_2_B", "Wall_Lean_2_2" },
        { "Wall_Vert_1_1_A", "Wall_Vert_1_1" },
        { "Wall_Vert_1_2_A", "Wall_Vert_1_2" },
        { "Wall_Vert_2_1_A", "Wall_Vert_2_1" },
        { "Wall_Vert_2_2_A", "Wall_Vert_2_2" }
    };

        private List<string> _keys;
        private string _folder;
        private GameObject _sourceMesh;
        private bool _skippingExistingPrefabs = false;

        [MenuItem("ZLD_Maker/Create prefabs from Theme Folder")]
        public static void ShowWindow()
        {
            SetMargings(3, 5, 0, 5);

            EditorWindow editorWindow = EditorWindow.GetWindow(typeof(CreatePrefabsFromThemeFolder));
            editorWindow.autoRepaintOnSceneChange = true;
            editorWindow.minSize = new Vector2(300 + margings.y + margings.w, 60 + margings.x + margings.z);
            editorWindow.titleContent = new GUIContent("Prefabs Creator");
            editorWindow.Show();
        }

        protected sealed override void MainSection()
        {
            _skippingExistingPrefabs = EditorGUILayout.Toggle(new GUIContent("Skipping existing prefabs"), _skippingExistingPrefabs);
            EditorGUILayout.Space(3);
            if (GUILayout.Button("Create Prefabs from Theme Selection", GUILayout.ExpandWidth(true)))
            {
                CreatePrefabsFromParent();
            }
        }

        public void CreatePrefabsFromParent()
        {
            var selection = Selection.assetGUIDs;

            if (!GetOnlyRootObject(selection)) return;

            var guidThemePath = AssetDatabase.GUIDToAssetPath(selection[0]);
            _folder = Path.GetFileName(guidThemePath);
            var modelsPath = $"{Models}{_folder}";
            var prefabsPath = $"{Prefabs}{_folder}";

            if (!Directory.Exists(prefabsPath))
            {
                bool createPrefabFolder = EditorUtility.DisplayDialog("Create Prefabs", "The prefabs folder doesn't exist! Do you want me to create the folder automatically?", "Yes", "No");

                if (createPrefabFolder)
                {
                    _ = Directory.CreateDirectory(prefabsPath);
                }
                else return;
            }

            string[] models = Directory.GetFiles(modelsPath, "*.fbx");
            _keys = _colliders.Keys.ToList();

            foreach (var model in models)
            {
                foreach (var key in _keys.ToList())
                {
                    var meshName = $"{MDL}{Struct}{_folder}_{key}";
                    Debug.Log(meshName);

                    if (model.Contains(meshName))
                    {
                        var prefabName = meshName.Replace(MDL, PRF);
                        var prefabPath = $"{prefabsPath}/{prefabName}.prefab";

                        Debug.Log(prefabPath);

                        if (File.Exists(prefabPath))
                        {
                            if (_skippingExistingPrefabs)
                            {
                                Debug.Log($"Prefab <color=orange>{prefabName}</color> <color=red>skipped</color>. This prefab already exists.");
                                break;
                            }
                        }

                        GameObject prefab = CreatePrefab(model, key, prefabName);
                        SavePrefab(prefab, prefabPath);

                        break;
                    }
                }
            }
        }

        private GameObject CreatePrefab(string model, string key, string prefabName)
        {
            GameObject prefab = new(prefabName);
            MeshCollider collider = prefab.AddComponent<MeshCollider>();

            var colliderValue = _colliders.FirstOrDefault(k => k.Key == key).Value;
            var meshCollider = string.Format($"{PathColliders}{MDL}{Collider}{colliderValue}{Extension}");

            GameObject _sourceCollider = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(meshCollider));

            collider.sharedMesh = _sourceCollider.GetComponent<MeshFilter>().sharedMesh;
            _sourceMesh = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(model));
            _sourceMesh.transform.parent = prefab.transform;

            _keys.Remove(key);

            DestroyImmediate(_sourceCollider);

            return prefab;
        }

        private static void SavePrefab(GameObject prefab, string prefabPath)
        {
            if (File.Exists(prefabPath))
            {
                PrefabUtility.SaveAsPrefabAsset(prefab, prefabPath);
                Debug.Log($"Pefab <color=orange>{prefab.name}</color> <color=cyan>replaced</color> successfully!!");
                DestroyImmediate(prefab);
            }
            else
            {
                string localPath = AssetDatabase.GenerateUniqueAssetPath(prefabPath);
                PrefabUtility.SaveAsPrefabAssetAndConnect(prefab, localPath, InteractionMode.UserAction, out bool prefabSuccess);

                if (prefabSuccess == true)
                {
                    Debug.Log($"Pefab <color=orange>{prefab.name}</color> <color=green>created</color> successfully!!");
                    DestroyImmediate(prefab);
                }
                else
                {
                    Debug.Log($"Can't create the prefab <color=orange>{prefab.name}</color>.");
                }
            }
        }

        private bool GetOnlyRootObject(string[] selection)
        {
            var rootIsOK = false;

            if (selection.Length == 0)
            {
                EditorUtility.DisplayDialog("Create Prefabs", "Nothing selected!\nPlease, select the Theme folder", "OK");
            }
            else
            {
                rootIsOK = true;
            }

            return rootIsOK;
        }
    }
}