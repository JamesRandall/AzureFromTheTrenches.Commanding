namespace AccidentalFish.Commanding.Abstractions
{
    public interface ICommandCorrelationIdProvider
    {
        string Create();
    }
}
