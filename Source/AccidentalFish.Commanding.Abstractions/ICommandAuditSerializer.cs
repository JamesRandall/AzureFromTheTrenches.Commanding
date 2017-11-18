using System.Threading.Tasks;

namespace AccidentalFish.Commanding
{
    public interface ICommandAuditSerializer
    {
        string Serialize<TCommand>(TCommand command) where TCommand : class;
    }
}
