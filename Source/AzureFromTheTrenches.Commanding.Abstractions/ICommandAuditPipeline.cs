using System;
using System.Threading;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions.Model;

namespace AzureFromTheTrenches.Commanding.Abstractions
{
    public interface ICommandAuditPipeline
    {
        Task AuditPreDispatch(ICommand command, ICommandDispatchContext dispatchContext, CancellationToken cancellationToken);
        Task AuditPostDispatch(ICommand command, ICommandDispatchContext dispatchContext, long elapsedMilliseconds, CancellationToken cancellationToken);
        Task AuditExecution(ICommand command, ICommandDispatchContext dispatchContext, long elapsedMilliseconds, bool executedSuccessfully, CancellationToken cancellationToken);
        Task Audit(AuditItem auditQueueItem, CancellationToken cancellationToken);
    }
}
