using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.Tests.Acceptance.Helpers
{
    class SimpleCommandWithNoResultHandler : ICommandHandler<SimpleCommandWithNoResult>
    {
        private readonly ICommandTracer _commandTracer;

        public SimpleCommandWithNoResultHandler(ICommandTracer commandTracer)
        {
            _commandTracer = commandTracer;
        }

        public Task ExecuteAsync(SimpleCommandWithNoResult command)
        {
            _commandTracer.Log("Executed SimpleCommandWithNoResultHandler");
            return Task.FromResult(0);
        }
    }
}
