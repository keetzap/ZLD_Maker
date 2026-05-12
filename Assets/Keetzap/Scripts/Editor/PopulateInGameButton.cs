using Keetzap.ZeldaMaker;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Keetzap.EditorTools
{
    public class PopulateInGameButton : BaseEditorWindow
    {
        private readonly string INGAMEBUTTON = "GenericInGameButton";
        private string _inGameButtonName;

        [MenuItem("Keetzap/Rename InGame Button")]
        public static void ShowWindow()
        {
            EditorWindow window = EditorWindow.GetWindow(typeof(PopulateInGameButton));
            window.maxSize = new Vector2(300, 55);
            window.minSize = new Vector2(220, 55);
            window.Show();
            window.titleContent = new GUIContent("Rename InGame Button");
        }

        void OnGUI()
        {
            _inGameButtonName = EditorGUILayout.TextField("InGame Button Name", _inGameButtonName);
            EditorGUILayout.Space(3);
            if (GUILayout.Button("Rename InGame Button", GUILayout.ExpandWidth(true), GUILayout.Height(22)))
            {
                RenameInGameButton();
            }
        }

        public void RenameInGameButton()
        {
            GameObject[] selectedObjects = Selection.gameObjects;

            if (selectedObjects.Length != 1)
            {
                EditorUtility.DisplayDialog("Invalid selection", "Select only a single object (specifically the root of the InGame Button), please!", "Ok");
                return;
            }

            if (selectedObjects[0].transform.GetChild(0).GetComponent<GenericButtonInGameController>() == null)
            {
                EditorUtility.DisplayDialog("Invalid selection", "Please, select an InGame Button.", "Ok");
                return;
            }

            Undo.RecordObject(selectedObjects[0], "Rename InGame Button");

            string newPrefabName = selectedObjects[0].name.Replace(INGAMEBUTTON, _inGameButtonName);
            selectedObjects[0].name = newPrefabName;

            GameObject button = selectedObjects[0].transform.Find(string.Format($"BTN_{INGAMEBUTTON}")).gameObject;
            string newButtonName = button.name.Replace(INGAMEBUTTON, _inGameButtonName);
            button.name = newButtonName;

            GameObject text = selectedObjects[0].transform.GetChild(0).transform.Find(string.Format($"TXT_{INGAMEBUTTON}")).gameObject;
            string newTextName = text.name.Replace(INGAMEBUTTON, _inGameButtonName);
            text.name = newTextName;
            text.GetComponent<TMP_Text>().text = _inGameButtonName;

            EditorUtility.SetDirty(selectedObjects[0]);
        }
    }
}