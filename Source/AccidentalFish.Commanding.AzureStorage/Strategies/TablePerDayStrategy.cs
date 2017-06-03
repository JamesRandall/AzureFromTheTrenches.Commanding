using System;
using System.Globalization;
using AccidentalFish.Commanding.AzureStorage.Model;

namespace AccidentalFish.Commanding.AzureStorage.Strategies
{
    public class TablePerDayStrategy : IStorageStrategy
    {
        private readonly string _byDateNameNamePrefix;
        private readonly string _byDateTableNamePostfix;
        private readonly string _byCorrelationIdTableNamePrefix;
        private readonly string _byCorrelationIdTableNamePostfix;

        public TablePerDayStrategy(string byDateNameNamePrefix = "ca", string byDateTableNamePostfix = "bydate", string byCorrelationIdTableNamePrefix = "ca", string byCorrelationIdTableNamePostfix = "bycorrelationid")
        {
            _byDateNameNamePrefix = byDateNameNamePrefix;
            _byDateTableNamePostfix = byDateTableNamePostfix;
            _byCorrelationIdTableNamePrefix = byCorrelationIdTableNamePrefix;
            _byCorrelationIdTableNamePostfix = byCorrelationIdTableNamePostfix;
        }

        public string GetTableName(CommandAuditByDateDescItem tableEntity)
        {
            return $"{_byDateNameNamePrefix}{tableEntity.DispatchedAtUtc.ToString("yyyyMMdd", CultureInfo.InvariantCulture)}{_byDateTableNamePostfix}";
        }

        public string GetTableName(CommandAuditByCorrelationIdItem tableEntity)
        {
            return $"{_byCorrelationIdTableNamePrefix}{tableEntity.DispatchedAtUtc.ToString("yyyyMMdd", CultureInfo.InvariantCulture)}{_byCorrelationIdTableNamePostfix}";
        }

        public string GetPartitionKey(CommandAuditByDateDescItem tableEntity)
        {
            return $"{tableEntity.DispatchedAtUtc:HHmm}";
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
