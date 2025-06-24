namespace Keetzap.Feedback
{
    public class FeedbackGeneralControlEditor : IFeedbackControl
    {
        private readonly FeedbackSystem _feedbackPlayer;
        
        public FeedbackGeneralControlEditor(FeedbackSystem feedbackPlayer)
        {
            _feedbackPlayer = feedbackPlayer;
        }
        public void Play()
        {
            _feedbackPlayer.PlayWithoutDelays();
        }

        public void Stop()
        {
            
        }

        public void Save()
        {
            
        }
    }
}