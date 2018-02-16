using System;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.Implementation
{
    class DefaultCommandExecutionExceptionHandler : ICommandExecutionExceptionHandler
    {
        public Task<bool> HandleException<TResult>(Exception ex, object handler, int handlerExecutionIndex, ICommand<TResult> command, ICommandDispatchContext dispatchContext)
        {
            throw new CommandExecutionException(command, handler?.GetType(), handlerExecutionIndex, dispatchContext?.Copy(), "Error occurred during command execution", ex);
        }
    }
}
