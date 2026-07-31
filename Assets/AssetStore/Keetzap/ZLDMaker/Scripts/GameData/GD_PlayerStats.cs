using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    [CreateAssetMenu(fileName = "PlayerStatus", menuName = "KTZ_ZeldaMaker/PlayerStatus", order = 1)]
    public class GD_PlayerStats : ScriptableObject
    {
        [Tooltip("Player Values")]
        [SerializeField] private int lifesMaxCapacity; //Change to HPMaxCapacity OJO!!! = 10
        [SerializeField] private int currentCapacity; //Change to HPMaxCapacity = 3
        [SerializeField] private int currentLifes; //Change to CurrentHP 
        [SerializeField] private int initialLifes; //Change to InitialHP OJO!! Potser no es necessita. = 1

        [SerializeField] private int gems;
        [SerializeField] private int silverKeys;
        [SerializeField] private int goldenKeys;
        [SerializeField] private int bossKey;

        [Header("Default Presset")]
        public int presetLifesMaxCapacity = 3;
        public int presetCurrentCapacity = 3;
        public int presetCurrentLifes = 3;
        public int presetInitialLifes = 3;
        public int presetGems = 0;
        public int presetSilverKeys = 0;
        public int presetGoldenKeys = 0;
        public int presetBossKey = 0;

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

        public void ApplyPreset()
        {
            lifesMaxCapacity = presetLifesMaxCapacity;
            currentCapacity = presetCurrentCapacity;
            currentLifes = presetCurrentLifes;
            initialLifes = presetInitialLifes;
            gems = presetGems;
            silverKeys = presetSilverKeys;
            goldenKeys = presetGoldenKeys;
            bossKey = presetBossKey;
        }
    }
}
