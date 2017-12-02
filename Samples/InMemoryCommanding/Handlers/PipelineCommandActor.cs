using System;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using InMemoryCommanding.Commands;

namespace InMemoryCommanding.Handlers
{
    class PipelineCommandActor : ICommandHandler<PipelineCommand, bool>
    {
        public Task<bool> ExecuteAsync(PipelineCommand command, bool previousResult)
        {
            Console.WriteLine("Pipeline command actor without cancellation token");
            return Task.FromResult(true);
        }
    }
}
