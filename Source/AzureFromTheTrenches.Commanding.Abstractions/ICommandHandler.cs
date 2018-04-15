using System.Threading;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions.Model;

namespace AzureFromTheTrenches.Commanding.Abstractions
{
    /// <summary>
    /// Can be implemented to provide basic command handling capabilites where no result is required
    /// </summary>
    /// <typeparam name="TCommand">The type of the command</typeparam>
    public interface ICommandHandler<in TCommand> : ICommandHandler where TCommand : ICommand
    {
        /// <summary>
        /// Executes the command
        /// </summary>
        /// <param name="command">The command</param>
        /// <returns>An awaitable task</returns>
        Task ExecuteAsync(TCommand command);
    }

    /// <summary>
    /// Can be implemented to provide basic command handling capabilites where no result is required and cancellation token support is required
    /// </summary>
    /// <typeparam name="TCommand">The type of the command</typeparam>
    public interface ICancellableCommandHandler<in TCommand> : ICancellableCommandHandler where TCommand : ICommand
    {
        /// <summary>
        /// Executes the command
        /// </summary>
        /// <param name="command">The command</param>
        /// <param name="cancellationToken">A cancellation tokan</param>
        /// <returns>An awaitable task</returns>
        Task ExecuteAsync(TCommand command, CancellationToken cancellationToken);
    }

    /// <summary>
    /// Can be implemented to provide basic command handling capabilites where a result is required
    /// </summary>
    /// <typeparam name="TCommand">The type of the command</typeparam>
    /// <typeparam name="TResult">The result type of the command</typeparam>
    public interface ICommandHandler<in TCommand, TResult> : ICommandHandler where TCommand : ICommand<TResult>
    {
        /// <summary>
        /// Executes the command
        /// </summary>
        /// <param name="command">The command</param>
        /// <param name="previousResult">The previous result set by the last (if any) command in the handler chain. Will be default(TResult) if the first command in the pipeline.</param>
        /// <returns>An awaitable task</returns>
        Task<TResult> ExecuteAsync(TCommand command, TResult previousResult);
    }

    /// <summary>
    /// Can be implemented to provide basic command handling capabilites where a result and cancellation token support is required
    /// </summary>
    /// <typeparam name="TCommand">The type of the command</typeparam>
    /// <typeparam name="TResult">The result type of the command</typeparam>
    public interface ICancellableCommandHandler<in TCommand, TResult> : ICancellableCommandHandler where TCommand : ICommand<TResult>
    {
        /// <summary>
        /// Executes the command
        /// </summary>
        /// <param name="command">The command</param>
        /// <param name="previousResult">The previous result set by the last (if any) command in the handler chain. Will be default(TResult) if the first command in the pipeline.</param>
        /// <param name="cancellationToken">A cancellation token</param>
        /// <returns>An awaitable task</returns>
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
        /// <summary>
        /// Executes the command
        /// </summary>
        /// <param name="command">The command</param>
        /// <param name="previousResult">The previous result set by the last (if any) command in the handler chain. Will be default(TResult) if the first command in the pipeline.</param>
        /// <returns>An awaitable task with the result wrapped in a class that allows the handler to halt execution</returns>
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
        /// <summary>
        /// Executes the command
        /// </summary>
        /// <param name="command">The command</param>
        /// <param name="previousResult">The previous result set by the last (if any) command in the handler chain. Will be default(TResult) if the first command in the pipeline.</param>
        /// <param name="cancellationToken">A cancellation token</param>
        /// <returns>An awaitable task with the result wrapped in a class that allows the handler to halt execution</returns>
        Task<PipelineAwareCommandHandlerResult<TResult>> ExecuteAsync(TCommand command, TResult previousResult, CancellationToken cancellationToken);
    }
}
