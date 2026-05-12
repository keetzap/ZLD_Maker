using System;
using UnityEngine;
using UnityEngine.UI;

namespace Keetzap.ZeldaMaker
{
    public class ConsumableView : MonoBehaviour
    {
        [SerializeField] private Image IMG_consumable;

        public TypeOfKey Key { get; set; }

        public void SetSprite(GD_Key key)
        {
            IMG_consumable.sprite = key.sprite;
        }
    }
}
