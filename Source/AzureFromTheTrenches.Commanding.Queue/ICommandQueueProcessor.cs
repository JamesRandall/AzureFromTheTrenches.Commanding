using System;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Queue.Model;

namespace AzureFromTheTrenches.Commanding.Queue
{
    public interface ICommandQueueProcessor
    {
        Task<bool> DequeueErrorHandler(Exception ex);

        Task<bool> HandleRecievedItemAsync<TCommand, TResult>(QueueItem<TCommand> item, int maxDequeueCount) where TCommand : class, ICommand<TResult>;
    }
}
