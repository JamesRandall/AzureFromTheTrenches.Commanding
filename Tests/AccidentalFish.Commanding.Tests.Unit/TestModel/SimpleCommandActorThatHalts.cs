using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;

namespace AzureFromTheTrenches.Commanding.Tests.Unit.TestModel
{
    class SimpleCommandActorThatHalts : ICommandChainActor<SimpleCommand, SimpleResult>
    {
        public Task<CommandChainActorResult<SimpleResult>> ExecuteAsync(SimpleCommand command, SimpleResult previousResult)
        {
            return Task.FromResult(new CommandChainActorResult<SimpleResult>(true, previousResult));
        }
    }
}
