using Keetzap.Core;
using System;
using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    [Serializable]
    public enum TypeOfCollectable
    {
        Heart,
        Life,
        Gem,
        Key
    }

    [Serializable]
    public enum TypeOfKey
    {
        SilverKey,
        GoldenKey,
        BossKey
    }
}