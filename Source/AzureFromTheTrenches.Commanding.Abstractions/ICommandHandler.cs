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

    public interface ICommandChainHandler : ICommandHandlerBase
    {
        
    }

    public interface ICommandHandler<in TCommand> : ICommandHandler where TCommand : ICommand
    {
        Task ExecuteAsync(TCommand command);
    }

    public interface ICommandHandler<in TCommand, TResult> : ICommandHandler where TCommand : ICommand<TResult>
    {
        Task<TResult> ExecuteAsync(TCommand command, TResult previousResult);
    }

    public interface ICommandChainHandler<in TCommand, TResult> : ICommandChainHandler where TCommand : ICommand<TResult>
    {
        // return true to stop after execution
        Task<CommandChainHandlerResult<TResult>> ExecuteAsync(TCommand command, TResult previousResult);
    }
}
