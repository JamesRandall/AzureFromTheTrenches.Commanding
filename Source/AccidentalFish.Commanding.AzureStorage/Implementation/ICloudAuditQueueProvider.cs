using Microsoft.WindowsAzure.Storage.Queue;

namespace AccidentalFish.Commanding.AzureStorage.Implementation
{
    internal interface ICloudAuditQueueProvider
    {
        CloudQueue Queue { get; }

        CloudQueue DeadLetterQueue { get; }
    }
}
