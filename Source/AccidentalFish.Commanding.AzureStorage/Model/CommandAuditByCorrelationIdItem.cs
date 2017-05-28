using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace AccidentalFish.Commanding.AzureStorage.Model
{
    internal class CommandAuditByCorrelationIdItem : TableEntity
    {
        public DateTime RecordedAtUtc { get; set; }

        public string CommandType { get; set; }

        public int Depth { get; set; }

        public Guid CommandId { get; set; }

        [IgnoreProperty]
        public string CorrelationId => PartitionKey;

        public static string GetPartitionKey(string correlationId)
        {
            return correlationId;
        }

        public static string GetRowKey(DateTime utcNow, Guid commandId)
        {
            return $"{DateTime.MaxValue.Ticks - utcNow.Ticks:D19}-{commandId}"; // we postfix the command ID to guarantee we don't have a date clash
        }
    }
}
