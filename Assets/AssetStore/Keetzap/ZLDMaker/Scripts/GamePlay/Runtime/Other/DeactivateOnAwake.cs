using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    public class DeactivateOnAwake : MonoBehaviour
    {
        void Awake()
        {
            if (gameObject.activeSelf)
            {
                gameObject.SetActive(false);
            }
        }
    }
}