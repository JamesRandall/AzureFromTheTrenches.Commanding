namespace AzureFromTheTrenches.Commanding.Queue
{
    /// <summary>
    /// Creates an instance of a back off policy
    /// </summary>
    public interface IAsynchronousBackoffPolicyFactory
    {
        IAsynchronousBackoffPolicy Create();
    }
}
