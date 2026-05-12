using System;
using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    interface IHitable
    {
        void OnAttacked(int damage, float power);
        GD_Hitable GameDataAsset();
        Action<int, float> OnAttackedEvent  { get; set; }
        Action<int, float> OnDeadEvent  { get; set; }
        Action<Hitable> OnDestructionEvent { get; set; }
    }
}