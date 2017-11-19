using AzureFromTheTrenches.Commanding.Abstractions;
using Microsoft.WindowsAzure.Storage.Queue;

namespace AzureFromTheTrenches.Commanding.AzureStorage
{
    public interface IAzureStorageQueueDispatcherFactory
    {
        ICommandDispatcher Create(CloudQueue queue);
    }
}
