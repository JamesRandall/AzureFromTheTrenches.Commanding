using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace AccidentalFish.Commanding.AzureStorage.Model
{
    internal class CommandAuditByDateDescItem : TableEntity
    {
        public DateTime RecordedAtUtc { get; set; }

        public string CommandType { get; set; }

        public string CorrelationId { get; set; }

        public int Depth { get; set; }

        [IgnoreProperty]
        public Guid CommandId => Guid.Parse(RowKey);

        public static string GetPartitionKey(DateTime utcNow)
        {
            return $"{DateTime.MaxValue.Ticks - utcNow.Ticks:D19}";
        }

        public static string GetRowKey(Guid commandId)
        {
            return commandId.ToString();
        }
    }
}
