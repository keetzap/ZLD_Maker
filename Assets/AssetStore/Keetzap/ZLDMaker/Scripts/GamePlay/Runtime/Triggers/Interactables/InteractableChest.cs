using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    public sealed class InteractableChest : InteractableTimeline
    {
        new public static class Fields
        {
            public static string Drop => nameof(drop);
            public static string GettingItemPosition => nameof(gettingItemPosition);
            public static string CollectableAnchorPosition => nameof(collectableAnchorPosition);
        }

        [SerializeField] private Collectable drop;
        [SerializeField] private Transform gettingItemPosition;
        [SerializeField] private Transform collectableAnchorPosition;

        public override void OnInteract()
        {
            if (_hasBeenUsed)
            {
                return;
            }

            if (drop == null)
            {
                Debug.LogError($"[InteractableChest] The 'Drop' field is not assigned on '{gameObject.name}'. Please assign a Collectable to it in the Inspector.", this);
                return;
            }

            base.OnInteract();

            drop.autodestroyObject = false;
            GameManager.Instance.AddCollectable(drop.configurationFile);
            Instantiate(drop.gameObject, collectableAnchorPosition.transform.position, collectableAnchorPosition.transform.rotation, collectableAnchorPosition.transform);

            OnInteractEnd();
        }

        public void SetGettingItemPosition()
        {
            PlayerController.Instance.MoveToTargetPosition(gettingItemPosition, _timeToRepositioning);
        }
    }
}