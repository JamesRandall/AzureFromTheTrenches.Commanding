using System;
using System.Threading;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions.Model;

namespace AzureFromTheTrenches.Commanding.Abstractions
{
    /// <summary>
    /// Represents an audit pipeline
    /// </summary>
    public interface ICommandAuditPipeline
    {
        /// <summary>
        /// Audit the command using the pre-dispatch pipeline
        /// </summary>
        /// <param name="command">The command to audit</param>
        /// <param name="dispatchContext">The dispatch context</param>
        /// <param name="cancellationToken">A cancalletion token</param>
        /// <returns>An awaitable task</returns>
        Task AuditPreDispatch(ICommand command, ICommandDispatchContext dispatchContext, CancellationToken cancellationToken);
        /// <summary>
        /// Audit the command using the post-dispatch pipeline
        /// </summary>
        /// <param name="command">The command to audit</param>
        /// <param name="dispatchContext">The dispatch context</param>
        /// <param name="elapsedMilliseconds">The number of milliseconds that it took to dispatch the command</param>
        /// <param name="cancellationToken">A cancellation token</param>
        /// <returns>An awaitbale task</returns>
        Task AuditPostDispatch(ICommand command, ICommandDispatchContext dispatchContext, long elapsedMilliseconds, CancellationToken cancellationToken);
        /// <summary>
        /// Audit the command after it's execution
        /// </summary>
        /// <param name="command">The command to audit</param>
        /// <param name="dispatchContext">The dispatch context</param>
        /// <param name="elapsedMilliseconds">The time taken to execute the command</param>
        /// <param name="executedSuccessfully">Whether or not the command was executed successfully</param>
        /// <param name="cancellationToken">A cancellation token</param>
        /// <returns>An awaitable task</returns>
        Task AuditExecution(ICommand command, ICommandDispatchContext dispatchContext, long elapsedMilliseconds, bool executedSuccessfully, CancellationToken cancellationToken);
        /// <summary>
        /// Store the supplied audit item - will get routed to the appropriate pipeline based on its type
        /// </summary>
        /// <param name="auditQueueItem">The audit item</param>
        /// <param name="cancellationToken">A cancellation token</param>
        /// <returns>An awaitable task</returns>
        Task Audit(AuditItem auditQueueItem, CancellationToken cancellationToken);
    }
}
