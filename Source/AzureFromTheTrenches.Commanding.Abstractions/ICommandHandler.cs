using System.Threading;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions.Model;

namespace AzureFromTheTrenches.Commanding.Abstractions
{
    public interface ICommandHandlerBase
    {

    }

    public interface ICommandHandler : ICommandHandlerBase
    {
        
    }

    public interface ICancellableCommandHandler : ICommandHandler
    {
        
    }

    public interface ICommandChainHandler : ICommandHandlerBase
    {
        
    }

    public interface ICancellableCommandChainHandler : ICommandChainHandler
    {

    }

    public interface ICommandHandler<in TCommand> : ICommandHandler where TCommand : ICommand
    {
        Task ExecuteAsync(TCommand command);
    }

    public interface ICancellableCommandHandler<in TCommand> : ICancellableCommandHandler where TCommand : ICommand
    {
        Task ExecuteAsync(TCommand command, CancellationToken cancellationToken);
    }

    public interface ICommandHandler<in TCommand, TResult> : ICommandHandler where TCommand : ICommand<TResult>
    {
        Task<TResult> ExecuteAsync(TCommand command, TResult previousResult);
    }

    public interface ICancellableCommandHandler<in TCommand, TResult> : ICancellableCommandHandler where TCommand : ICommand<TResult>
    {
        Task<TResult> ExecuteAsync(TCommand command, TResult previousResult, CancellationToken cancellationToken);
    }

    public interface ICommandChainHandler<in TCommand, TResult> : ICommandChainHandler where TCommand : ICommand<TResult>
    {
        // return true to stop after execution
        Task<CommandChainHandlerResult<TResult>> ExecuteAsync(TCommand command, TResult previousResult);
    }

    public interface ICancellableCommandChainHandler<in TCommand, TResult> : ICancellableCommandChainHandler where TCommand : ICommand<TResult>
    {
        // return true to stop after execution
        Task<CommandChainHandlerResult<TResult>> ExecuteAsync(TCommand command, TResult previousResult, CancellationToken cancellationToken);
    }
}
