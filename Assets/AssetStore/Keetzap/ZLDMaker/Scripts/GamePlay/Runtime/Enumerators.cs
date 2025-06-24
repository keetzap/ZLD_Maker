namespace Keetzap.ZeldaMaker
{
    public enum TypeOfAttackEvent
    {
        OnlyOnDamageEvent,
        OnlyOnDeadEvent,
        OnEveryAttackEvent
    }

    public enum TypeOfDestruction
    {
        Instantly,
        AfterFeedbackDuration,
        AfterDelay
    }

    public enum TypeOfMessage
    {
        Hint,
        Information,
        Dialog,
        Extended
    }
}
