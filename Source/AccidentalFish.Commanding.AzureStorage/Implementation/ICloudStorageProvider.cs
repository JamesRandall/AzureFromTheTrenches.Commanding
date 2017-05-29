using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;

namespace AccidentalFish.Commanding.AzureStorage.Implementation
{
    interface ICloudStorageProvider
    {
        Task<CloudTable> GetTable(string tableName);

        Task<CloudBlobContainer> GetBlobContainer();
    }
}
