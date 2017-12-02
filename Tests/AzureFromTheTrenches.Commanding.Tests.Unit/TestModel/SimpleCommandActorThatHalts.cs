using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;

namespace AzureFromTheTrenches.Commanding.Tests.Unit.TestModel
{
    class SimplePipelineAwareCommandHandlerThatHalts : IPipelineAwareCommandHandler<SimpleCommand, SimpleResult>
    {
        public Task<PipelineAwareCommandHandlerResult<SimpleResult>> ExecuteAsync(SimpleCommand command, SimpleResult previousResult)
        {
            return Task.FromResult(new PipelineAwareCommandHandlerResult<SimpleResult>(true, previousResult));
        }
    }
}
