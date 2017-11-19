namespace AzureFromTheTrenches.Commanding.Queue.Implementation
{
    internal class AsynchronousBackoffPolicyFactory : IAsynchronousBackoffPolicyFactory
    {
        public IAsynchronousBackoffPolicy Create()
        {
            return new AsynchronousBackoffPolicy();
        }
    }
}
