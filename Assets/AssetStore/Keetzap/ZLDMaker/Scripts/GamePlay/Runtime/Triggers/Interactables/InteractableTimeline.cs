using Keetzap.Feedback;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

namespace Keetzap.ZeldaMaker
{
    public class InteractableTimeline : Interactable
    {
        new public static class Fields
        {
            public static string InitialTimeline => nameof(initialTimeline);
            public static string DefaultInitialPosition => nameof(defaultInitialPosition);
            public static string TimeToRepositioning => nameof(timeToRepositioning);
            public static string OpeningTimeline => nameof(openingTimeline);
            public static string ClosingTimeline => nameof(closingTimeline);
            public static string DialogMessage => nameof(dialogMessage);
            public static string ExitFeedback => nameof(exitFeedback);
            public static string EndInteraction => nameof(endInteraction);
        }

        [SerializeField] private PlayableDirector initialTimeline;
        [SerializeField] private Transform defaultInitialPosition;
        [SerializeField] private float timeToRepositioning = 10;
        [SerializeField] private PlayableDirector openingTimeline;
        [SerializeField] private PlayableDirector closingTimeline;
        [SerializeField] private string dialogMessage;
        [SerializeField] private FeedbackSystem exitFeedback;
        [SerializeField] private UnityEvent endInteraction;

        private bool isWaitingForClosing;
        protected float _timeToRepositioning => timeToRepositioning;

        private void Start()
        {
            if (initialTimeline != null)
            {
                initialTimeline.Play();
            }
        }

        public override void OnInteract()
        {
            base.OnInteract();

            PlayerController.Instance.MoveToTargetPosition(defaultInitialPosition, timeToRepositioning);
            PauseCharacter();
            HideWeapons();
            DestroyAfterInteraction();
        }

        protected void OnInteractEnd()
        {
            CanvasController.Instance.CloseMessageEvent += OnCloseMessage;
            openingTimeline.Play();
            isWaitingForClosing = true;
        }

        public void ShowTimelineMessage()
        {
            CanvasController.Instance.DisplayMessage(TypeOfMessage.Information, dialogMessage);
        }

        protected override void OnCloseMessage()
        {
            if (isWaitingForClosing)
            {
                base.OnCloseMessage();

                isWaitingForClosing = false;

                openingTimeline.Stop();
                closingTimeline.Play();

                if (exitFeedback != null)
                {
                    exitFeedback.Play();
                }

                endInteraction?.Invoke();
            }
        }
    }
}