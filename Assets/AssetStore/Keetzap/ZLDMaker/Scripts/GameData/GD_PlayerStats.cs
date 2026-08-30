using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    [CreateAssetMenu(fileName = "PlayerStatus", menuName = "KTZ_ZeldaMaker/PlayerStatus", order = 1)]
    public class GD_PlayerStats : ScriptableObject
    {
        public static class Fields
        {
            public static string LifesMaxCapacity => nameof(lifesMaxCapacity);
            public static string CurrentCapacity => nameof(currentCapacity);
            public static string CurrentLifes => nameof(currentLifes);
            public static string InitialLifes => nameof(initialLifes);

            public static string Gems => nameof(gems);
            public static string SilverKeys => nameof(silverKeys);
            public static string GoldenKeys => nameof(goldenKeys);
            public static string BossKey => nameof(bossKey);

            public static string PresetLifesMaxCapacity => nameof(presetLifesMaxCapacity);
            public static string PresetCurrentCapacity => nameof(presetCurrentCapacity);
            public static string PresetCurrentLifes => nameof(presetCurrentLifes);
            public static string PresetInitialLifes => nameof(presetInitialLifes);
            public static string PresetGems => nameof(presetGems);
            public static string PresetSilverKeys => nameof(presetSilverKeys);
            public static string PresetGoldenKeys => nameof(presetGoldenKeys);
            public static string PresetBossKey => nameof(presetBossKey);
        }

        [Tooltip("Player Values")]
        [SerializeField] private int lifesMaxCapacity = 3;
        [SerializeField] private int currentCapacity = 3;
        [SerializeField] private int currentLifes = 3;
        [SerializeField] private int initialLifes = 3;

        [SerializeField] private int gems = 0;
        [SerializeField] private int silverKeys = 0;
        [SerializeField] private int goldenKeys = 0;
        [SerializeField] private int bossKey = 0;

        [Header("Preset Values")]
        [SerializeField] private int presetLifesMaxCapacity = 3;
        [SerializeField] private int presetCurrentCapacity = 3;
        [SerializeField] private int presetCurrentLifes = 3;
        [SerializeField] private int presetInitialLifes = 3;

        [SerializeField] private int presetGems = 0;
        [SerializeField] private int presetSilverKeys = 0;
        [SerializeField] private int presetGoldenKeys = 0;
        [SerializeField] private int presetBossKey = 0;

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
