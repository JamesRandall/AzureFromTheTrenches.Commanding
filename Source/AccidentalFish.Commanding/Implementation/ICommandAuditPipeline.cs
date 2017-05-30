using System.Threading.Tasks;

namespace AccidentalFish.Commanding.Implementation
{
    internal interface ICommandAuditPipeline
    {
        void RegisterAuditor<TAuditorImpl>() where TAuditorImpl : ICommandAuditor;
        Task Audit<TCommand>(TCommand command, ICommandDispatchContext dispatchContext) where TCommand : class;
    }
}
