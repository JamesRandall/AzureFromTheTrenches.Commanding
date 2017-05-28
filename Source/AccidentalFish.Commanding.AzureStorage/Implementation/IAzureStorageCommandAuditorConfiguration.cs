using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;

namespace AccidentalFish.Commanding.AzureStorage.Implementation
{
    interface IAzureStorageCommandAuditorConfiguration
    {
        CloudTable AuditByCorrelationIdTable { get; }

        CloudTable AuditByDateDescTable { get; }

        CloudBlobContainer CommandPayloadContainer { get; }
    }
}
