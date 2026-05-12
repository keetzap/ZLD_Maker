using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    public sealed class InteractableSign : Interactable
    {
        new public static class Fields
        {
            public static string TypeOfMessage => nameof(typeOfMessage);
            public static string Message => nameof(message);
        }

        [SerializeField] private string message;
        [SerializeField] private TypeOfMessage typeOfMessage;

        public override void OnInteract()
        {
            base.OnInteract();

            PauseCharacter();
            HideWeapons();
            DestroyAfterInteraction();

            CanvasController.Instance.DisplayMessage(typeOfMessage, message);
        }
    }
}