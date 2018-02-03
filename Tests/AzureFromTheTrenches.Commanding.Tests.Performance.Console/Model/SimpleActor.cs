using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.Tests.Performance.Console.Model
{
    class SimpleHandler : ICommandHandler<SimpleCommand, SimpleResult>
    {
        public Task<SimpleResult> ExecuteAsync(SimpleCommand command, SimpleResult previousResult)
        {
            return Task.FromResult(previousResult);
        }
    }

    class SimpleHandlerNoResult : ICommandHandler<SimpleCommandNoResult>
    {
        public Task ExecuteAsync(SimpleCommandNoResult command)
        {
            return Task.FromResult(0);
        }
    }
}
