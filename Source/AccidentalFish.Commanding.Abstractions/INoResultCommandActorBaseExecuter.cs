using System.Threading.Tasks;

namespace AzureFromTheTrenches.Commanding.Abstractions
{
    /// <summary>
    /// Implementations of this interface are able to execute commands when no result type is specified
    /// </summary>
    public interface INoResultCommandActorBaseExecuter
    {
        Task ExecuteAsync(object actorInstance, object command);
    }
}
