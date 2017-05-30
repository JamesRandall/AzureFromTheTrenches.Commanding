using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;

namespace AccidentalFish.Commanding.AzureStorage.Implementation
{
    internal interface ICloudQueueProvider
    {
        CloudQueue Queue { get; }

        CloudBlobContainer BlobContainer { get; }
    }
}
