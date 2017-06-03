using System;
using System.Globalization;
using AccidentalFish.Commanding.AzureStorage.Model;

namespace AccidentalFish.Commanding.AzureStorage.Strategies
{
    public class SingleTableStrategy : IStorageStrategy
    {
        private readonly string _byDateDescTableName;
        private readonly string _byCorrelationIdTableName;

        public SingleTableStrategy(string byDateDescTableName = "commandauditbydate", string byCorrelationIdTableName = "commandauditbycorrelationid")
        {
            _byDateDescTableName = byDateDescTableName;
            _byCorrelationIdTableName = byCorrelationIdTableName;
        }

        public string GetTableName(CommandAuditByDateDescItem tableEntity)
        {
            return _byDateDescTableName;
        }

        public string GetTableName(CommandAuditByCorrelationIdItem tableEntity)
        {
            return _byCorrelationIdTableName;
        }

        public string GetPartitionKey(CommandAuditByDateDescItem tableEntity)
        {
            return tableEntity.DispatchedAtUtc.ToString("yyyyMMddHH", CultureInfo.InvariantCulture);
        }

        public string GetRowKey(CommandAuditByDateDescItem tableEntity)
        {
            return $"{DateTime.MaxValue.Ticks - tableEntity.DispatchedAtUtc.Ticks:D19}-{tableEntity.CommandId}";
        }

        public string GetPartitionKey(CommandAuditByCorrelationIdItem tableEntity)
        {
            return tableEntity.CorrelationId;
        }

        public string GetRowKey(CommandAuditByCorrelationIdItem tableEntity)
        {
            return $"{DateTime.MaxValue.Ticks - tableEntity.DispatchedAtUtc.Ticks:D19}-{tableEntity.CommandId}";
        }
    }
}
