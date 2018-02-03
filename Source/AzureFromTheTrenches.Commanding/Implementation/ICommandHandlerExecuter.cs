using System;
using System.Threading;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.Implementation
{
    interface ICommandHandlerExecuter
    {
        Task<TResult> ExecuteAsync<TResult>(ICommandHandler handler, ICommand<TResult> command, TResult previousResult, CancellationToken cancellationToken);
        void CompileHandlerExecuter(Type commandType, Type commandHandlerType);
    }
}
