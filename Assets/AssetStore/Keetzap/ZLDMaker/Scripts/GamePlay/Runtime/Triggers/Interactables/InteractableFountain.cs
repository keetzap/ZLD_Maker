using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    public sealed class InteractableFountain : InteractableTimeline
    {
        new public static class Fields
        {
            public static string RefillCompletely => nameof(refillCompletely);
            public static string NumberOfLifes => nameof(numberOfLifes);
        }

        [SerializeField] private bool refillCompletely;
        [SerializeField] private int numberOfLifes = 3;

        public override void OnInteract()
        {
            if (_hasBeenUsed)
            {
                return;
            }

            base.OnInteract();

            int lifes = refillCompletely ? GameManager.Instance.GameData.playerStats.LifesMaxCapacity : numberOfLifes;
            GameManager.Instance.SetLife(lifes);
            
            OnInteractEnd();

        }
    }
}