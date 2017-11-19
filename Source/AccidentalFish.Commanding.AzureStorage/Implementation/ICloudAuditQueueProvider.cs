using Microsoft.WindowsAzure.Storage.Queue;

namespace AzureFromTheTrenches.Commanding.AzureStorage.Implementation
{
    internal interface ICloudAuditQueueProvider
    {
        CloudQueue Queue { get; }

        CloudQueue DeadLetterQueue { get; }
    }
}
