using System;
using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    interface IListener
    {
        void OnTrigger(object sender, Collider other);
    }
}