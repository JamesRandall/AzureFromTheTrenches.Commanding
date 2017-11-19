using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureFromTheTrenches.Commanding.AzureStorage.Implementation
{
    /// <summary>
    /// This is a little naive at the moment as it doesn't remove old entries that haven't been accessed for some time.
    /// As some of the storage strategies create table names based on data it exists to prevent repeated calls to CreateIfNotExists
    /// </summary>
    internal class CloudStorageProvider : ICloudStorageProvider
    {
        private readonly CloudTableClient _tableClient;
        private readonly CloudBlobContainer _commandPayloadContainer;
        private readonly ConcurrentDictionary<string, CloudTable> _cloudTables = new ConcurrentDictionary<string, CloudTable>();
        private volatile bool _hasAttemptedCreation;

        public CloudStorageProvider(CloudTableClient tableClient, CloudBlobContainer commandPayloadContainer)
        {
            _tableClient = tableClient;
            _commandPayloadContainer = commandPayloadContainer;
        }

        public async Task<CloudTable> GetTable(string tableName)
        {
            CloudTable cloudTable;
            if (!_cloudTables.TryGetValue(tableName, out cloudTable))
            {
                cloudTable = _tableClient.GetTableReference(tableName);
                if (_cloudTables.TryAdd(tableName, cloudTable))
                {
                    // it doesn't matter if we call this more than once we're just trying to avoid a network hop
                    // unless we have to
                    await cloudTable.CreateIfNotExistsAsync();
                }                
            }

            return cloudTable;            
        }

        public async Task<CloudBlobContainer> GetBlobContainer()
        {
            if (!_hasAttemptedCreation)
            {
                // we don't care too much if this gets called a couple of times, it will only be early in execution and longer term
                // saving is significant
                _hasAttemptedCreation = true;
                await _commandPayloadContainer.CreateIfNotExistsAsync();
            }
            return _commandPayloadContainer;
        }
    }
}
