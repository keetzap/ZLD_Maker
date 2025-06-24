using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    [CreateAssetMenu(fileName = "Consumable", menuName = "KTZ_ZeldaMaker/Items/Consumable")]
    public class GD_Collectable : GD_AssetBase
    {
        [Space]
        public TypeOfCollectable typeOfCollectable;
        public int amount;
    }
}