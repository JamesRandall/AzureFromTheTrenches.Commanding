using System;
using System.Threading.Tasks;
using AccidentalFish.Commanding.Abstractions.Model;

namespace AccidentalFish.Commanding.Abstractions
{
    public interface ICommandAuditPipeline
    {
        Task Audit<TCommand>(TCommand command, Guid commandId, ICommandDispatchContext dispatchContext) where TCommand : class;
        Task Audit(AuditItem auditItem);
    }
}
