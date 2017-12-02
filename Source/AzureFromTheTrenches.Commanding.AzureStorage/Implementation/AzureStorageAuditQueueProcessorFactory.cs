using System;
using System.Threading;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;
using AzureFromTheTrenches.Commanding.Queue;
using AzureFromTheTrenches.Commanding.Queue.Model;
using Microsoft.WindowsAzure.Storage.Queue;

namespace AzureFromTheTrenches.Commanding.AzureStorage.Implementation
{
    internal class AzureStorageAuditQueueProcessorFactory : IAzureStorageAuditQueueProcessorFactory
    {
        private readonly IAsynchronousBackoffPolicyFactory _backoffPolicyFactory;
        private readonly IAzureStorageQueueSerializer _serializer;
        private readonly ICommandAuditPipeline _commandAuditPipeline;
        private readonly ICloudAuditQueueProvider _cloudAuditQueueProvider;

        public AzureStorageAuditQueueProcessorFactory(IAsynchronousBackoffPolicyFactory backoffPolicyFactory,
            IAzureStorageQueueSerializer serializer,
            ICommandAuditPipeline commandAuditPipeline,
            ICloudAuditQueueProvider cloudAuditQueueProvider)
        {
            _backoffPolicyFactory = backoffPolicyFactory;
            _serializer = serializer;
            _commandAuditPipeline = commandAuditPipeline;
            _cloudAuditQueueProvider = cloudAuditQueueProvider;
        }

        public Task Start(CancellationToken cancellationToken, CloudQueue deadLetterQueue = null, int maxDequeueCount = 10, Action<string> traceLogger = null)
        {
            AzureStorageQueueBackoffProcessor<AuditItem> queueProcessor = new AzureStorageQueueBackoffProcessor<AuditItem>(
                _backoffPolicyFactory.Create(),
                _serializer,
                _cloudAuditQueueProvider.Queue,
                item => HandleRecievedItemAsync(_cloudAuditQueueProvider.DeadLetterQueue, item, maxDequeueCount),
                traceLogger,
                HandleError);
            return queueProcessor.StartAsync(cancellationToken);
        }

        private Task<bool> HandleError(Exception arg)
        {
            return Task.FromResult(false);
        }

        private async Task<bool> HandleRecievedItemAsync(CloudQueue deadLetterQueue, QueueItem<AuditItem> item, int maxDequeueCount)
        {
            AuditItem auditQueueItem = item.Item;
            try
            {
                await _commandAuditPipeline.AuditPreDispatch(auditQueueItem);

                return true;
            }
            catch (Exception)
            {
                if (item.DequeueCount > maxDequeueCount && deadLetterQueue != null)
                {
                    try
                    {
                        string json = _serializer.Serialize(auditQueueItem);
                        await deadLetterQueue.AddMessageAsync(new CloudQueueMessage(json));
                        return true;
                    }
                    catch (Exception)
                    {
                        return false;
                    }                    
                }
                return false;
            }            
        }
    }
}
