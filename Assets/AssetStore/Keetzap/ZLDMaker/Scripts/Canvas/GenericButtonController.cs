using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System;

namespace Keetzap.ZeldaMaker
{
    public class GenericButtonController : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        [SerializeField] private GameObject Floriture;
        [SerializeField] private TMP_Text ButtonText;
        [SerializeField] private Color TextColorWhenSelected = Color.white;
        [SerializeField] private Color TextColorWhenDeselected = Color.white;

        public void OnSelect()
        {
            Floriture.SetActive(true);
            ButtonText.color = TextColorWhenSelected;
        }

        public void OnSelect(BaseEventData eventData)
        {
            OnSelect();
        }

        public void OnDeselect(BaseEventData eventData)
        {
            Floriture.SetActive(false);
            ButtonText.color = TextColorWhenDeselected;
        }
    }
}
