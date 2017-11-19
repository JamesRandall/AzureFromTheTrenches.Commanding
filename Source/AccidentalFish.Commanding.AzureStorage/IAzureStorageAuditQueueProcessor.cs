using System.Threading;
using System.Threading.Tasks;

namespace AzureFromTheTrenches.Commanding.AzureStorage
{
    /// <summary>
    /// Represents a queue processor that pulls items from the audit queue and processes them into
    /// the configured auditors
    /// </summary>
    public interface IAzureStorageAuditQueueProcessor
    {
        Task Start(CancellationTokenSource cancellationTokenSource);
    }
}
