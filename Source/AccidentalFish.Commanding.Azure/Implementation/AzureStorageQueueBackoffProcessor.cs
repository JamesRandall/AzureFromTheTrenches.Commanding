using System;
using System.Threading;
using System.Threading.Tasks;
using AccidentalFish.Commanding.Queue;
using AccidentalFish.Commanding.Queue.Model;
using Microsoft.WindowsAzure.Storage.Queue;

namespace AccidentalFish.Commanding.AzureStorage.Implementation
{
    internal class AzureStorageQueueBackoffProcessor<T> where T : class
    {
        private readonly Func<Exception, Task<bool>> _dequeErrorHandler;
        private readonly IAsynchronousBackoffPolicy _backoffPolicy;
        private readonly IAzureStorageQueueCommandSerializer _serializer;
        private readonly CloudQueue _queue;
        private readonly Action<string> _logger;
        private readonly Func<QueueItem<T>, Task<bool>> _handleReceivedItemAsyncFunc;

        public AzureStorageQueueBackoffProcessor(
            IAsynchronousBackoffPolicy backoffPolicy,
            IAzureStorageQueueCommandSerializer serializer,
            CloudQueue queue,
            Func<QueueItem<T>, Task<bool>> handleReceivedItemAsyncFunc,
            Action<string> logger = null,
            Func<Exception, Task<bool>> dequeErrorHandlerFunc = null)
        {
            _backoffPolicy = backoffPolicy;
            _serializer = serializer;
            _queue = queue;
            _logger = logger;
            _handleReceivedItemAsyncFunc = handleReceivedItemAsyncFunc;
            _dequeErrorHandler = dequeErrorHandlerFunc;
        }
        
        public Task StartAsync(CancellationToken token)
        {
            return _backoffPolicy.ExecuteAsync(AttemptDequeueAsync, token);            
        }

        private async Task<bool> AttemptDequeueAsync()
        {
            try
            {
                try
                {
                    CloudQueueMessage message = await _queue.GetMessageAsync();
                    if (message != null)
                    {
                        QueueItem<T> item = new QueueItem<T>(_serializer.Deserialize<T>(message.AsString),
                            message.DequeueCount,
                            message.PopReceipt,
                            null,
                            () => _queue.UpdateMessageAsync(message, TimeSpan.FromSeconds(30), MessageUpdateFields.Visibility));
                        bool shouldPop = await ProcessItem(item);
                        if (shouldPop)
                        {
                            await _queue.DeleteMessageAsync(message);
                        }
                        return true;
                    }                    
                    return false;
                }
                catch (Exception ex)
                {
                    if (_dequeErrorHandler != null)
                    {
                        return await _dequeErrorHandler(ex);
                    }
                    throw;
                }
            }
            catch (Exception)
            {
                _logger?.Invoke("Error occurred in dequeue");
                throw;
            }
        }

        private async Task<bool> ProcessItem(QueueItem<T> message)
        {
            if (message == null)
            {
                return false;
            }            

            return await _handleReceivedItemAsyncFunc(message);
        }
    }
}
