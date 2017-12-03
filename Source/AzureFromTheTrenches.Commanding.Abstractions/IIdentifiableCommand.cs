namespace AzureFromTheTrenches.Commanding.Abstractions
{
    public interface IIdentifiableCommand
    {
        string CommandId { get; }
    }
}
