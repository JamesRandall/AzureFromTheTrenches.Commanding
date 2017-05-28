using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;

namespace AccidentalFish.Commanding.AzureStorage.Implementation
{
    class AzureStorageCommandAuditorConfiguration : IAzureStorageCommandAuditorConfiguration
    {
        public AzureStorageCommandAuditorConfiguration(CloudTable auditByCorrelationIdTable,
            CloudTable auditByDateDescTable, CloudBlobContainer commandPayloadContainer)
        {
            AuditByCorrelationIdTable = auditByCorrelationIdTable;
            AuditByDateDescTable = AuditByDateDescTable;
            CommandPayloadContainer = commandPayloadContainer;
        }

        public CloudTable AuditByCorrelationIdTable { get; }
        public CloudTable AuditByDateDescTable { get; }
        public CloudBlobContainer CommandPayloadContainer { get; }
    }
}
