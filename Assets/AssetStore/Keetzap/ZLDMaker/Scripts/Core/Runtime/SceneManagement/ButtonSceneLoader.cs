using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ButtonSceneLoader : MonoBehaviour, ISerializationCallbackReceiver
{
    #if UNITY_EDITOR
    [SerializeField] private SceneAsset sceneToLoad;
    #endif

    [HideInInspector, SerializeField] private string sceneName;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(LoadScene);
    }

    public void LoadScene()
    {
        if (!SceneExistsInBuildSettings(sceneName))
        {
            Debug.LogError($"Scene '{sceneName}' is not added to Build Settings!");
            return;
        }

        SceneManager.LoadScene(sceneName);
    }

    public void OnBeforeSerialize()
    {
        #if UNITY_EDITOR
        if (sceneToLoad != null)
        {
            sceneName = sceneToLoad.name;
        }
        #endif
    }

    public void OnAfterDeserialize() { }

    private bool SceneExistsInBuildSettings(string sceneNameToCheck)
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;

        for (int i = 0; i < sceneCount; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string name = System.IO.Path.GetFileNameWithoutExtension(path);

            if (name == sceneNameToCheck)
            {
                return true;
            }
        }

        return false;
    }
}