using UnityEngine;
using UnityEngine.UI;

namespace Keetzap.ZeldaMaker
{
    public class HeartView : MonoBehaviour
    {
        [SerializeField] private Image IMG_heart;

        public void SetLife(bool visibility)
        {
            IMG_heart.enabled = visibility;
        }
    }
}
