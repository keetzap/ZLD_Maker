using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    public sealed class InteractableChest : InteractableTimeline
    {
        new public static class Fields
        {
            public static string Drop => nameof(drop);
            public static string DefaultGettingItemPosition => nameof(defaultGettingItemPosition);
            public static string DropAnchorPosition => nameof(dropAnchorPosition);
        }

        [SerializeField] private GameObject drop;
        [SerializeField] private Transform defaultGettingItemPosition;
        [SerializeField] private Transform dropAnchorPosition;

        public override void OnInteract()
        {
            if (_hasBeenUsed)
            {
                return;
            }

            base.OnInteract();

            drop.GetComponent<Collectable>().autodestroyObject = false;
            GameManager.Instance.AddCollectable(drop.GetComponent<Collectable>().configurationFile);
            Instantiate(drop, dropAnchorPosition.transform.position, dropAnchorPosition.transform.rotation, dropAnchorPosition.transform);

            OnInteractEnd();
        }

        public void SetGettingItemPosition()
        {
            PlayerController.Instance.MoveToTargetPosition(defaultGettingItemPosition, _timeToRepositioning);
        }
    }
}