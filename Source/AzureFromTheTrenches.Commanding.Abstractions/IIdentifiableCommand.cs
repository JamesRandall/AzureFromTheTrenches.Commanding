namespace AzureFromTheTrenches.Commanding.Abstractions
{
    public interface IIdentifiableCommand
    {
        string Id { get; }
    }
}
