using System;
using System.Threading.Tasks;

namespace AccidentalFish.Commanding.Implementation
{
    internal interface ICommandAuditPipeline
    {
        void RegisterAuditor<TAuditorImpl>() where TAuditorImpl : ICommandAuditor;
        Task Audit<TCommand>(TCommand command, Guid commandId, ICommandDispatchContext dispatchContext) where TCommand : class;
    }
}
