using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    [CreateAssetMenu(fileName = "EnemyBase", menuName = "KTZ_ZeldaMaker/Enemies", order = 0)]
    public class GD_Enemy : GD_Hitable, IGameDataAsset
    {
        [Space(10)]
        [Header("Unalerted properties")]
        public float unalertedRangeOfSight = 5;
        public Color unalertedRangeOfSightColor = new(1, 0.7f, 0, 0.6f);
        public float unalertedRangeOfAttack = 1;
        public Color unalertedRangeOfAttackColor = new(1, 0.2f, 0, 0.8f);
        public float unalertedAngleOfAction = 120;
        public Color unalertedAngleOfActionColor = new(0.2f, 0.2f, 1);
        public float unalertedSpeed = 0.5f;
        [Space(10)]
        [Header("Alerted properties")]
        public float alertedRangeOfSight = 7;
        public Color alertedRangeOfSightColor = new(1, 0.7f, 0, 0.6f);
        public float alertedRangeOfAttack = 1;
        public Color alertedRangeOfAttackColor = new(1, 0.2f, 0, 0.8f);
        public float alertedAngleOfAction = 360;
        public Color alertedAngleOfActionColor = new(0.2f, 0.2f, 1);
        public float alertedSpeed = 1;
        [Space(10)]
        [Header("Other properties")]
        public float stuntTime = 1.5f;
        public float recoil = 1;
        public float recoilSpeed = 5;

        public void OnResetValues()
        {
        }
    }
}