using System.Threading.Tasks;

namespace AccidentalFish.Commanding
{
    public interface ICommandAuditor
    {
        Task Audit<TCommand>(TCommand command, ICommandDispatchContext dispatchContext) where TCommand : class;
    }
}
