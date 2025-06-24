using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour, ISerializationCallbackReceiver
{
    [SerializeField] private Object mainMenuScene;
    [Space(5)]
    [SerializeField] private Slider loading;
    [SerializeField] private float loadingSpeed = 2f;
    [HideInInspector] public string nameScene;
    
    public void OnAfterDeserialize() { }

    public void OnBeforeSerialize()
    {
        nameScene = mainMenuScene.name;
    }

    void Update()
    {
        loading.value += Time.deltaTime / loadingSpeed;

        if (loading.value >= 1)
        {
            SceneManager.LoadScene(nameScene);
        }
    }
}
