using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;

namespace AzureFromTheTrenches.Commanding.Tests.Unit.TestModel
{
    class SimpleCommandHandlerThatHalts : ICommandChainHandler<SimpleCommand, SimpleResult>
    {
        public Task<CommandChainHandlerResult<SimpleResult>> ExecuteAsync(SimpleCommand command, SimpleResult previousResult)
        {
            return Task.FromResult(new CommandChainHandlerResult<SimpleResult>(true, previousResult));
        }
    }
}
