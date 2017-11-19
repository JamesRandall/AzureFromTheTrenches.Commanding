using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureFromTheTrenches.Commanding.AzureStorage.Model
{
    public class AbstractCommandAuditTableEntity : TableEntity
    {
        public DateTime DispatchedAtUtc { get; set; }

        public string CommandType { get; set; }

        public string CorrelationId { get; set; }

        public int Depth { get; set; }

        public Guid CommandId { get; set; }

        [IgnoreProperty]
        public IReadOnlyDictionary<string, string> AdditionalProperties { get; set; }

        public override IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            var results = base.WriteEntity(operationContext);
            if (AdditionalProperties != null)
            {
                HashSet<string> existingProperties = new HashSet<string>(results.Keys);
                foreach (var kvp in AdditionalProperties)
                {
                    string keyName = existingProperties.Contains(kvp.Key) ? $"e{kvp.Key}" : kvp.Key;
                    results.Add(keyName, new EntityProperty(kvp.Value));
                }
            }
            return results;
        }
    }
}
