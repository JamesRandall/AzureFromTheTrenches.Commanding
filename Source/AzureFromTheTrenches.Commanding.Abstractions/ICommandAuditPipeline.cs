using System;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions.Model;

namespace AzureFromTheTrenches.Commanding.Abstractions
{
    public interface ICommandAuditPipeline
    {
        Task AuditDispatch(ICommand command, ICommandDispatchContext dispatchContext);
        Task AuditDispatch(AuditItem auditItem);
        Task AuditExecution(ICommand command, ICommandDispatchContext dispatchContext, bool executedSuccessfully);
        Task AuditExecution(AuditItem auditItem);
    }
}
