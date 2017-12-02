using System;
using System.Threading;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;
using InMemoryCommanding.Commands;

namespace InMemoryCommanding.Handlers
{
    class CancellablePipelineCommandActor : ICancellableCommandChainHandler<PipelineCommand, bool>
    {
        public Task<CommandChainHandlerResult<bool>> ExecuteAsync(PipelineCommand command, bool previousResult, CancellationToken cancellationToken)
        {
            Console.WriteLine("Pipeline command actor with cancellation token");
            return Task.FromResult(new CommandChainHandlerResult<bool>(false, true));
        }
    }
}
