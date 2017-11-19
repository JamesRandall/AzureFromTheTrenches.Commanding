using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue;

namespace AzureFromTheTrenches.Commanding.AzureStorage
{
    public interface IAzureStorageAuditQueueProcessorFactory
    {
        Task Start(CancellationToken cancellationToken, CloudQueue deadLetterQueue = null, int maxDequeueCount = 10, Action<string> traceLogger = null);
    }
}
