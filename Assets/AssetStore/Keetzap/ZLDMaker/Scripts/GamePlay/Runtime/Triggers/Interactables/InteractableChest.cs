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

        [SerializeField] private GameObject drop;
        [SerializeField] private Transform gettingItemPosition;
        [SerializeField] private Transform collectableAnchorPosition;

        public override void OnInteract()
        {
            if (_hasBeenUsed)
            {
                return;
            }

            base.OnInteract();

            drop.GetComponent<Collectable>().autodestroyObject = false;
            GameManager.Instance.AddCollectable(drop.GetComponent<Collectable>().configurationFile);
            Instantiate(drop, collectableAnchorPosition.transform.position, collectableAnchorPosition.transform.rotation, collectableAnchorPosition.transform);

            OnInteractEnd();
        }

        public void SetGettingItemPosition()
        {
            PlayerController.Instance.MoveToTargetPosition(gettingItemPosition, _timeToRepositioning);
        }
    }
}