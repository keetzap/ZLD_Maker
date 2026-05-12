using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Keetzap.ZeldaMaker
{
    public class GenericButtonInGameController : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        [SerializeField] private Image Border;
        [SerializeField] private Color BorderColorWhenSelected = Color.white;
        [SerializeField] private Color BorderColorWhenDeselected = Color.white;
        [SerializeField] private TMP_Text ButtonText;
        [SerializeField] private Color TextColorWhenSelected = Color.white;
        [SerializeField] private Color TextColorWhenDeselected = Color.white;

        public void OnSelect()
        {
            Border.color = BorderColorWhenSelected;
            ButtonText.color = TextColorWhenSelected;
        }

        public void OnDeselect()
        {
            Border.color = BorderColorWhenDeselected;
            ButtonText.color = TextColorWhenDeselected;
        }

        public void OnSelect(BaseEventData eventData)
        {
            OnSelect();
        }

        public void OnDeselect(BaseEventData eventData)
        {
            OnDeselect();
        }
    }
}
