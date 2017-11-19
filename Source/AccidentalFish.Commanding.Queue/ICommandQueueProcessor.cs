using System;
using System.Threading.Tasks;
using AccidentalFish.Commanding.Abstractions;
using AccidentalFish.Commanding.Queue.Model;

namespace AccidentalFish.Commanding.Queue
{
    public interface ICommandQueueProcessor
    {
        Task<bool> DequeueErrorHandler(Exception ex);

        Task<bool> HandleRecievedItemAsync<TCommand, TResult>(QueueItem<TCommand> item, int maxDequeueCount) where TCommand : class, ICommand<TResult>;
    }
}
