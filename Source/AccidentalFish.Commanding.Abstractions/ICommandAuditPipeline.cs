using System;
using System.Threading.Tasks;
using AccidentalFish.Commanding.Abstractions.Model;

namespace AccidentalFish.Commanding.Abstractions
{
    public interface ICommandAuditPipeline
    {
        Task Audit(ICommand command, Guid commandId, ICommandDispatchContext dispatchContext);
        Task Audit(AuditItem auditItem);
    }
}
