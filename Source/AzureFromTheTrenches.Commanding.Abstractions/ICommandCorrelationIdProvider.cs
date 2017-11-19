namespace AzureFromTheTrenches.Commanding.Abstractions
{
    public interface ICommandCorrelationIdProvider
    {
        string Create();
    }
}
