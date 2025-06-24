using System;

namespace Keetzap.Feedback
{
    public sealed class FeedbackElementControlsEditor : IFeedbackControl
    {
        private readonly FeedbackEffect _feedbackEffect;
        
        public FeedbackElementControlsEditor(FeedbackEffect feedbackEffect)
        {
            _feedbackEffect = feedbackEffect;
        }

        public void Play()
        {
            _feedbackEffect.PlayWithoutDelay();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            FeedbackSaveState.SaveChangesToPersistant(_feedbackEffect);
        }
    }
}