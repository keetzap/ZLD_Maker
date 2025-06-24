using System.Collections.Generic;
using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    public class ConsumableController : MonoBehaviour
    {
        [SerializeField] private GameObject consumablePrefab;
        [SerializeField] private List<ConsumableView> consumables = new();

        public void SetKey(GD_Key key, int value)
        {
            if (value > 0)
            {
                CreateConsumable(key);
            }
            else
            {
                DestroyConsumable(key);
            }
        }

        public void CreateConsumable(GD_Key key)
        {
            GameObject consumable = Instantiate(consumablePrefab, transform);

            ConsumableView view = consumable.GetComponent<ConsumableView>();
            view.Key = key.typeOfkey;
            view.SetSprite(key);

            consumables.Add(view);
        }

        private void DestroyConsumable(GD_Key key)
        {
            for (int c = 0; c < consumables.Count; c++)
            {
                if (consumables[c].Key == key.typeOfkey)
                {
                    Destroy(consumables[c].gameObject);
                    consumables.Remove(consumables[c]);
                    break;
                }
            }
        }
    }
}
