using System.Threading;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions.Model;

namespace AzureFromTheTrenches.Commanding.Abstractions
{
    #region Base interfaces for type safety in implementation and generic support
    public interface ICommandHandlerBase
    {

    }

    public interface ICommandHandler : ICommandHandlerBase
    {
        
    }

    public interface ICancellableCommandHandler : ICommandHandler
    {
        
    }

    public interface IPipelineAwareCommandHandler : ICommandHandlerBase
    {
        
    }

    public interface ICancellablePipelineAwareCommandHandler : IPipelineAwareCommandHandler
    {

    }
    #endregion

    /// <summary>
    /// Can be implemented to provide basic command handling capabilites where no result is required
    /// </summary>
    /// <typeparam name="TCommand">The type of the command</typeparam>
    public interface ICommandHandler<in TCommand> : ICommandHandler where TCommand : ICommand
    {
        Task ExecuteAsync(TCommand command);
    }

    /// <summary>
    /// Can be implemented to provide basic command handling capabilites where no result is required and cancellation token support is required
    /// </summary>
    /// <typeparam name="TCommand">The type of the command</typeparam>
    public interface ICancellableCommandHandler<in TCommand> : ICancellableCommandHandler where TCommand : ICommand
    {
        Task ExecuteAsync(TCommand command, CancellationToken cancellationToken);
    }

    /// <summary>
    /// Can be implemented to provide basic command handling capabilites where a result is required
    /// </summary>
    /// <typeparam name="TCommand">The type of the command</typeparam>
    /// <typeparam name="TResult">The result type of the command</typeparam>
    public interface ICommandHandler<in TCommand, TResult> : ICommandHandler where TCommand : ICommand<TResult>
    {
        Task<TResult> ExecuteAsync(TCommand command, TResult previousResult);
    }

    /// <summary>
    /// Can be implemented to provide basic command handling capabilites where a result and cancellation token support is required
    /// </summary>
    /// <typeparam name="TCommand">The type of the command</typeparam>
    /// <typeparam name="TResult">The result type of the command</typeparam>
    public interface ICancellableCommandHandler<in TCommand, TResult> : ICancellableCommandHandler where TCommand : ICommand<TResult>
    {
        Task<TResult> ExecuteAsync(TCommand command, TResult previousResult, CancellationToken cancellationToken);
    }

    /// <summary>
    /// Can be implemented to provide basic command handling capabilites where a result is required and the handler wishes to be
    /// able to halt the command handler execution pipeline
    /// </summary>
    /// <typeparam name="TCommand">The type of the command</typeparam>
    /// <typeparam name="TResult">The result type of the command</typeparam>
    public interface IPipelineAwareCommandHandler<in TCommand, TResult> : IPipelineAwareCommandHandler where TCommand : ICommand<TResult>
    {
        // return true to stop after execution
        Task<PipelineAwareCommandHandlerResult<TResult>> ExecuteAsync(TCommand command, TResult previousResult);
    }

    /// <summary>
    /// Can be implemented to provide basic command handling capabilites where a result and cancellation token support is required
    /// and the handler wishes to be able to halt the command handler execution pipeline
    /// </summary>
    /// <typeparam name="TCommand">The type of the command</typeparam>
    /// <typeparam name="TResult">The result type of the command</typeparam>
    public interface ICancellablePipelineAwareCommandHandler<in TCommand, TResult> : ICancellablePipelineAwareCommandHandler where TCommand : ICommand<TResult>
    {
        // return true to stop after execution
        Task<PipelineAwareCommandHandlerResult<TResult>> ExecuteAsync(TCommand command, TResult previousResult, CancellationToken cancellationToken);
    }
}
