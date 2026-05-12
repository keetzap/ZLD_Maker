using Keetzap.EditorTools;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;

namespace Keetzap.ZLD_Maker.Tools
{
    public class CreatePrefabsFromSingleMeshes : BaseEditorWindow
    {
        private const string MDL = "MDL_";
        private const string PRF = "PRF_";
        private const string Models = "Models";
        private const string Prefabs = "Prefabs";

        private string _sourcefolder;
        private string _destinationfolder;

        private GameObject _prefab;
        private GameObject _sourceMesh;
        private bool _skippingExistingPrefabs = false;

        [MenuItem("ZLD_Maker/Create prefabs from Single Meshes")]
        public static void ShowWindow()
        {
            EditorWindow window = EditorWindow.GetWindow(typeof(CreatePrefabsFromSingleMeshes));
            ShowWindow(window, "Prefabs Creator", 3, 5, 0, 5, 300, 60, true);
        }

        protected sealed override void MainSection()
        {
            _skippingExistingPrefabs = EditorGUILayout.Toggle(new GUIContent("Skipping existing prefabs"), _skippingExistingPrefabs);
            EditorGUILayout.Space(3);
            if (GUILayout.Button("Create Prefabs from Folder", GUILayout.ExpandWidth(true)))
            {
                _sourcefolder = GetSourceFolder();
                if (!string.IsNullOrEmpty(_sourcefolder))
                {
                    _destinationfolder = CreateDestinationFolder(_sourcefolder);
                    CreatePrefabsFromFolder();
                }
            }
        }

        public static string GetSourceFolder()
        {
            Object[] selection = Selection.GetFiltered(typeof(DefaultAsset), SelectionMode.Assets);

            if (selection.Length != 1)
            {
                bool createPrefabFolder = EditorUtility.DisplayDialog(
                        "Create Prefabs",
                        "Please select exactly ONE folder in the Project window.",
                        "Yes");
                return null;
            }

            return AssetDatabase.GetAssetPath(selection[0]);
        }

        private static string CreateDestinationFolder(string path)
        {
            string prefabsPath = string.Empty;

            if (path.Contains(Models))
            {
                prefabsPath = path.Replace(Models, Prefabs);

                if (!AssetDatabase.IsValidFolder(prefabsPath))
                {
                    bool createPrefabFolder = EditorUtility.DisplayDialog(
                        "Create Prefabs",
                        "The prefabs folder doesn't exist! Do you want me to create the folder automatically?",
                        "Yes",
                        "No");

                    if (createPrefabFolder)
                    {
                        string parent = Path.GetDirectoryName(prefabsPath);
                        string newFolder = Path.GetFileName(prefabsPath);

                        AssetDatabase.CreateFolder(parent, newFolder);
                        AssetDatabase.Refresh();
                    }
                    else
                    {
                        Debug.LogWarning("The Prefabs folder does not exist and was not created.");
                        return null;
                    }
                }
            }
            else
            {
                Debug.LogWarning($"The path does not contain '{Models}'. No replacement was made.");
            }

            return prefabsPath;
        }


        public void CreatePrefabsFromFolder()
        {
            string[] models = Directory.GetFiles(_sourcefolder, "*.fbx");

            foreach (var model in models)
            {
                string mesh = Path.GetFileName(model);

                Debug.Log(model);

                var prefabName = mesh.Replace(MDL, PRF);
                var prefabPath = $"{_destinationfolder}/{prefabName}.prefab";

                if (File.Exists(prefabPath))
                {
                    if (_skippingExistingPrefabs)
                    {
                        Debug.Log($"Prefab <color=orange>{prefabName}</color> <color=red>skipped</color>. This prefab already exists.");
                        continue;
                    }
                }
                else
                {
                    _prefab = new(prefabName);
                    _sourceMesh = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(model));
                    _sourceMesh.transform.parent = _prefab.transform;

                    SavePrefab(_prefab, prefabPath);
                }
            }
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
    }
}