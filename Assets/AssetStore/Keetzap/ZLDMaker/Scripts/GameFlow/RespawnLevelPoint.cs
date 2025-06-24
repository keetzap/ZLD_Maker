using System;
using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    [RequireComponent(typeof(BoxCollider))]
    public class RespawnLevelPoint : RespawnPoint
    {
        public Action<RespawnLevelPoint> OnDropByEvent;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(StringsData.PLAYER))
            {
                OnDropByEvent?.Invoke(this);
            }
        }
    }   
}
