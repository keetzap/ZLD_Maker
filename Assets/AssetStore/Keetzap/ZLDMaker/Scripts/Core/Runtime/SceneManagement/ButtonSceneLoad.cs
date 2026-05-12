using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonSceneLoad : MonoBehaviour, ISerializationCallbackReceiver
{
    [SerializeField] private Object sceneToLoad;
    [HideInInspector] public string nameScene;

    public void OnAfterDeserialize() { }

    public void OnBeforeSerialize()
    {
        nameScene = sceneToLoad.name;
    }

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(LoadScene);
    }

    private void LoadScene()
    {
        SceneManager.LoadScene(nameScene);
    }
}
