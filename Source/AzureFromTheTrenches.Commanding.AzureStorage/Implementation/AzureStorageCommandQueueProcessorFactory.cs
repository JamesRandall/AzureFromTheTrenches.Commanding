using System;
using System.Threading;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Queue;
using Microsoft.WindowsAzure.Storage.Queue;

namespace AzureFromTheTrenches.Commanding.AzureStorage.Implementation
{
    internal class AzureStorageCommandQueueProcessorFactory : IAzureStorageCommandQueueProcessorFactory
    {
        private readonly ICommandQueueProcessor _commandQueueProcessor;
        private readonly IAsynchronousBackoffPolicyFactory _backoffPolicyFactory;
        private readonly IAzureStorageQueueSerializer _serializer;

        public AzureStorageCommandQueueProcessorFactory(ICommandQueueProcessor commandQueueProcessor,
            IAsynchronousBackoffPolicyFactory backoffPolicyFactory,
            IAzureStorageQueueSerializer serializer)
        {
            _commandQueueProcessor = commandQueueProcessor;
            _backoffPolicyFactory = backoffPolicyFactory;
            _serializer = serializer;
        }

        public Task Start<TCommand,TResult>(CloudQueue queue, CancellationToken cancellationToken, int maxDequeueCount = 10, Action<string> traceLogger = null) where TCommand : class, ICommand<TResult>
        {
            AzureStorageQueueBackoffProcessor<TCommand> queueProcessor = new AzureStorageQueueBackoffProcessor<TCommand>(
                _backoffPolicyFactory.Create(),
                _serializer,
                queue,
                item => _commandQueueProcessor.HandleRecievedItemAsync<TCommand, TResult>(item, maxDequeueCount),
                traceLogger,
                _commandQueueProcessor.DequeueErrorHandler);
            return queueProcessor.StartAsync(cancellationToken);
        }

        public Task Start<TCommand, TResult>(CloudQueue queue, int maxDequeueCount = 10, Action<string> traceLogger = null) where TCommand : class, ICommand<TResult>
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            return Start<TCommand, TResult>(queue, cancellationTokenSource.Token, maxDequeueCount, traceLogger);
        }
    }
}
