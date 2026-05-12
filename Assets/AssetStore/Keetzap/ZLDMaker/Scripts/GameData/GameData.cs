using System.Collections.Generic;
using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "GameData", menuName = "KTZ_ZeldaMaker/GameData", order = 1)]
    public class GameData : ScriptableObject
    {
        public GD_PlayerStats playerStats;
        public GD_Weapon leftWeapon;
        public List<GD_Collectable> collectables;
        public List<GD_Key> keys;
    }
}