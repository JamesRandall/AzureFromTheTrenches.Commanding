using System;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.Tests.Unit.TestModel
{
    internal class ThrowExceptionCommandHandler : ICommandHandler<SimpleCommand, SimpleResult>
    {
        public Task<SimpleResult> ExecuteAsync(SimpleCommand command, SimpleResult previousResult)
        {
            throw new InvalidOperationException();
        }
    }
}
