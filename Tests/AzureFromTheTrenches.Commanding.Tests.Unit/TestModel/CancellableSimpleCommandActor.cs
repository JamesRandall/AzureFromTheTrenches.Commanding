using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.Tests.Unit.TestModel
{
    internal class CancellableSimpleCommandHandler : ICancellableCommandHandler<SimpleCommand, SimpleResult>
    {
        public Task<SimpleResult> ExecuteAsync(SimpleCommand command, SimpleResult previousResult, CancellationToken cancellationToken)
        {
            SimpleResult result = new SimpleResult
            {
                Handlers = new List<Type> { GetType() }
            };
            return Task.FromResult(result);
        }
    }
}
