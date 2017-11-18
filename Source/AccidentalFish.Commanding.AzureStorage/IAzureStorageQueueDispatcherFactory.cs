using AccidentalFish.Commanding.Abstractions;
using Microsoft.WindowsAzure.Storage.Queue;

namespace AccidentalFish.Commanding.AzureStorage
{
    public interface IAzureStorageQueueDispatcherFactory
    {
        ICommandDispatcher Create(CloudQueue queue);
    }
}
