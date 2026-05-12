using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    [CreateAssetMenu(fileName = "PlayerStatus", menuName = "KTZ_ZeldaMaker/PlayerStatus", order = 1)]
    public class GD_PlayerStats : ScriptableObject
    {
        //[Serializable]
        //public struct Collectable
        //{
        //    public GD_Collectable collectable;
        //    public int amount;
        //}

        //[SerializeField] private List<Collectable> collectables;
        //private Dictionary<GD_Collectable, int> _collectables = new Dictionary<GD_Collectable, int>();

        //public void InitializeCollectables()
        //{
        //    for (int c = 0; c < collectables.Count; c++)
        //    {
        //        _collectables.Add(collectables[c].collectable, collectables[c].amount);
        //    }
        //}

        [Tooltip("dsfsdfsdfs")]
        [SerializeField] private int lifesMaxCapacity; //Change to HPMaxCapacity OJO!!! = 10


        [SerializeField] private int currentCapacity; //Change to HPMaxCapacity = 3
        [SerializeField] private int currentLifes; //Change to CurrentHP 
        
        
        [SerializeField] private int initialLifes; //Change to InitialHP OJO!! Potser no es necessita. = 1
        // Com a molt, potser dinàmic (floor & ceil de la meitat ???)


        [SerializeField] private int gems;
        [SerializeField] private int silverKeys;
        [SerializeField] private int goldenKeys;
        [SerializeField] private int bossKey;

        //public int Hearts
        //{
        //    get => _collectables.FirstOrDefault(c => c.Key.typeOfCollectable == TypeOfCollectable.Heart).Value;
        //    set { _collectables.FirstOrDefault(c => c.Key.typeOfCollectable == TypeOfCollectable.Heart); }
        //}

        public int LifesMaxCapacity
        {
            get => lifesMaxCapacity;
            set { lifesMaxCapacity = value; }
        }

        public void SetLifes(int amount)
        {
            currentLifes += amount;
            currentLifes = Math.Clamp(currentLifes, 0, lifesMaxCapacity);
        }

        public int GetCurrentLifes() => currentLifes;

        public int Gems
        {
            get => gems;
            set { gems = value; }
        }

        public int SilverKey
        {
            get => silverKeys;
            set { silverKeys = value; }
        }

        public int GoldenKey
        {
            get => goldenKeys;
            set { goldenKeys = value; }
        }

        public int BossKey
        {
            get => bossKey;
            set { bossKey = value; }
        }

        public void AddingHeart()
        {
            LifesMaxCapacity++;
            currentLifes = LifesMaxCapacity;
        }

        public int GetKeyAmount(TypeOfKey key)
        {
            return key switch
            {
                TypeOfKey.SilverKey => SilverKey,
                TypeOfKey.GoldenKey => GoldenKey,
                TypeOfKey.BossKey => BossKey,
                _ => 0
            };
        }

        public void SetKeyAmount(TypeOfKey key, int value)
        {
            switch (key)
            {
                case TypeOfKey.SilverKey:   SilverKey += value; break;
                case TypeOfKey.GoldenKey:   GoldenKey += value; break;
                case TypeOfKey.BossKey:     BossKey += value;   break;
            }
        }

        public void InitializeLifes()
        {
            currentLifes = initialLifes;
        }
    }
}