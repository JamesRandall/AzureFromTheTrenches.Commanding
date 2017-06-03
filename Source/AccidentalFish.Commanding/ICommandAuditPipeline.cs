using System;
using System.Threading.Tasks;
using AccidentalFish.Commanding.Model;

namespace AccidentalFish.Commanding
{
    public interface ICommandAuditPipeline
    {
        Task Audit<TCommand>(TCommand command, Guid commandId, ICommandDispatchContext dispatchContext) where TCommand : class;
        Task Audit(AuditItem auditItem);
    }
}
