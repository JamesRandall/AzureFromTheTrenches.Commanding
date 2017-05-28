using System.Threading.Tasks;

namespace AccidentalFish.Commanding
{
    public interface ICommandAuditor
    {
        Task Audit<TCommand>(TCommand command, ICommandContext context) where TCommand : class;
    }
}
