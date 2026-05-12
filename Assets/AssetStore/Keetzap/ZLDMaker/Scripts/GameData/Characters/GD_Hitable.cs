using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    [CreateAssetMenu(fileName = "EnemyBase", menuName = "KTZ_ZeldaMaker/Attackable", order = 0)]
    public class GD_Hitable : GD_AssetBase
    {
        // Use 0 for infinity
        [Space(20)]
        public float life;
    }
}