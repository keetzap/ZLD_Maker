using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    [CreateAssetMenu(fileName = "Key", menuName = "KTZ_ZeldaMaker/Items/Consumable/Key")]
    public class GD_Key : GD_Collectable
    {
        [Space]
        public TypeOfKey typeOfkey;
    }
}