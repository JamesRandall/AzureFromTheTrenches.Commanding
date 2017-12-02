using System.Threading;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;

namespace AzureFromTheTrenches.Commanding.Implementation
{
    internal interface IPipelineAwareCommandHandlerExecuter
    {
        Task<PipelineAwareCommandHandlerResult<TResult>> ExecuteAsync<TResult>(IPipelineAwareCommandHandler handler, ICommand<TResult> command, TResult previousResult, CancellationToken cancellationToken);
    }
}