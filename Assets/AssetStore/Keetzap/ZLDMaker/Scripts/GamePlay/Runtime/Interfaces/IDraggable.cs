using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    interface IDraggable
    {
        public void OnStartDragging(PlayerController characterController);
        public bool OnDragged();
        public bool OnPushed();
        public void OnStopBehaviour();
    }
}