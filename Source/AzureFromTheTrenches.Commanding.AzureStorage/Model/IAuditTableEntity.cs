using System;

namespace AzureFromTheTrenches.Commanding.AzureStorage.Model
{
    public interface IAuditTableEntity
    {
        DateTime RecordedAtUtc { get; }

        string CommandType { get; }

        int Depth { get; }

        string CorrelationId { get; }
    }
}
