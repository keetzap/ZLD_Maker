using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "KTZ_ZeldaMaker/Items/Weapon")]
    public class GD_Weapon : GD_AssetBase, IGameDataAsset
    {
        private int _damage = 100;
        private int _cadence = 0;
        private float _power = 1;
        [Space(20)]
        public int damage;
        public int cadence;
        public float power;

        public void OnResetValues()
        {
            damage = _damage;
            cadence = _cadence;
            power = _power;
        }
    }
}