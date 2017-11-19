using System;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions.Model;

namespace AzureFromTheTrenches.Commanding.Abstractions
{
    public interface ICommandAuditPipeline
    {
        Task Audit(ICommand command, Guid commandId, ICommandDispatchContext dispatchContext);
        Task Audit(AuditItem auditItem);
    }
}
