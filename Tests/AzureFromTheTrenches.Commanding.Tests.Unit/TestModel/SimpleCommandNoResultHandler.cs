using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.Tests.Unit.TestModel
{
    class SimpleCommandNoResultHandler : ICommandHandler<SimpleCommandNoResult>
    {
        public Task ExecuteAsync(SimpleCommandNoResult command)
        {
            command.WasHandled = true;
            return Task.FromResult(0);
        }
    }
}
