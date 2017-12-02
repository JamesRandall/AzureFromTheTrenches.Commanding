using System.Threading;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.Tests.Unit.TestModel
{
    class CancellableSimpleCommandNoResultHandler : ICancellableCommandHandler<SimpleCommandNoResult>
    {
        public Task ExecuteAsync(SimpleCommandNoResult command, CancellationToken cancellationToken)
        {
            command.WasHandled = true;
            return Task.FromResult(0);
        }
    }
}
