using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.Tests.Unit.TestModel
{
    internal class SimpleCommandHandler : ICommandHandler<SimpleCommand, SimpleResult>
    {
        public Task<SimpleResult> ExecuteAsync(SimpleCommand command, SimpleResult previousResult)
        {
            SimpleResult result = new SimpleResult
            {
                Handlers = new List<Type> {GetType()}
            };
            return Task.FromResult(result);
        }
    }
}
