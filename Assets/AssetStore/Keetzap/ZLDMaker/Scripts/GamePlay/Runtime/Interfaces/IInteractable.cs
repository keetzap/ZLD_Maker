using System;
using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    interface IInteractable
    {
        Action OnInteractEvent { get; set; }

        void OnInteract();
    }
}