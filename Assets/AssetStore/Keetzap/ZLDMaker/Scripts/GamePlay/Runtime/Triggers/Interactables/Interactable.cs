using Keetzap.Feedback;
using System;
using UnityEngine;

namespace Keetzap.ZeldaMaker
{
    public abstract class Interactable : Trigger, IInteractable
    {
        new public static class Fields
        {
            public static string ShowHintMessage => nameof(showHintMessage);
            public static string HintMessage => nameof(hintMessage);
            public static string InteractFeedback => nameof(interactFeedback);
            public static string PauseCharacter => nameof(pauseCharacter);
            public static string HideWeapons => nameof(hideWeapons);
            public static string DestroyAfterInteraction => nameof(destroyAfterInteraction);
        }

        public Action OnInteractEvent { get; set; }

        [Tooltip("This property shows a message when the player gets cloer esdfsdfsdf")]
        [SerializeField] private bool showHintMessage = true;
        
        [Tooltip("This property shows a message when the player gets cloer esdfsdfsdf")]
        [SerializeField] private string hintMessage;
        
        [Tooltip("This property shows a message when the player gets cloer esdfsdfsdf")]
        [SerializeField] private FeedbackSystem interactFeedback;
        [SerializeField] private bool pauseCharacter;
        [SerializeField] private bool hideWeapons;
        [SerializeField] private bool destroyAfterInteraction;

        public override void Awake()
        {
            base.Awake();

            CanvasController.Instance.CloseMessageEvent += OnCloseMessage;
        }

        public virtual void OnInteract()
        {
            OnInteractEvent?.Invoke();

            if (interactFeedback != null)
            {
                interactFeedback.Play();
            }

            HideActionMessage();
        }

        protected void PauseCharacter()
        {
            if (pauseCharacter)
            {
                PlayerController.Instance.PauseInteractions();
            }
        }

        protected void HideWeapons()
        {
            if (hideWeapons)
            {
                PlayerController.Instance.HideWeapons();
            }
        }

        protected void DestroyAfterInteraction()
        {
            if (destroyAfterInteraction)
            {
                _hasBeenUsed = true;
            }
        }

        public virtual void OnTriggerEnter(Collider other)
        {
            if (_hasBeenUsed)
            {
                return;
            }
            else
            {
                if (showHintMessage && other.CompareTag(StringsData.PLAYER))
                {
                    CanvasController.Instance.DisplayMessage(TypeOfMessage.Hint, hintMessage);
                }
            }
        }

        public virtual void OnTriggerExit(Collider other)
        {
            if (!_hasBeenUsed && showHintMessage && other.CompareTag(StringsData.PLAYER))
            {
                HideActionMessage();
            }
        }

        protected void HideActionMessage()
        {
            CanvasController.Instance.HideMessage(TypeOfMessage.Hint);
        }

        protected virtual void OnCloseMessage()
        {
            if (hideWeapons)
            {
                PlayerController.Instance.ShowWeapons();
            }

            if (pauseCharacter)
            {
                PlayerController.Instance.ResumeInteractions();
            }
        }

        private void OnDestroy()
        {
            CanvasController.Instance.CloseMessageEvent -= OnCloseMessage;
        }
    }
}