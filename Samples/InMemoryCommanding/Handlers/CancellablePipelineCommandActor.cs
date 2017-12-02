using System;
using System.Threading;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;
using InMemoryCommanding.Commands;

namespace InMemoryCommanding.Handlers
{
    class CancellablePipelineAwarePipelineCommandActor : ICancellablePipelineAwareCommandHandler<PipelineCommand, bool>
    {
        public Task<PipelineAwareCommandHandlerResult<bool>> ExecuteAsync(PipelineCommand command, bool previousResult, CancellationToken cancellationToken)
        {
            Console.WriteLine("Pipeline command actor with cancellation token");
            return Task.FromResult(new PipelineAwareCommandHandlerResult<bool>(false, true));
        }
    }
}
