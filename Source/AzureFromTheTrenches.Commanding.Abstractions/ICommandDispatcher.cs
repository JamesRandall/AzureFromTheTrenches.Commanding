using System.Threading;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions.Model;

namespace AzureFromTheTrenches.Commanding.Abstractions
{
    /// <summary>
    /// This is the primary interface for applications to dispatch commands through.
    /// <inheritdoc />
    /// </summary>
    public interface ICommandDispatcher : IFrameworkCommandDispatcher
    {
        
    }
}
