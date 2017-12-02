using System;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions.Model;

namespace AzureFromTheTrenches.Commanding.Abstractions
{
    public interface ICommandAuditPipeline
    {
        Task AuditPreDispatch(ICommand command, ICommandDispatchContext dispatchContext);
        Task AuditPreDispatch(AuditItem auditItem);
        Task AuditPostDispatch(ICommand command, ICommandDispatchContext dispatchContext);
        Task AuditPostDispatch(AuditItem auditItem);
        Task AuditExecution(ICommand command, ICommandDispatchContext dispatchContext, bool executedSuccessfully);
        Task AuditExecution(AuditItem auditItem);
    }
}
