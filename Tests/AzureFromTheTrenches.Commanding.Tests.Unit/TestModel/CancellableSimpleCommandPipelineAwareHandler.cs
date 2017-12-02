using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;

namespace AzureFromTheTrenches.Commanding.Tests.Unit.TestModel
{
    class CancellableSimpleCommandPipelineAwareHandler : ICancellablePipelineAwareCommandHandler<SimpleCommand, SimpleResult>
    {
        public Task<PipelineAwareCommandHandlerResult<SimpleResult>> ExecuteAsync(SimpleCommand command, SimpleResult previousResult, CancellationToken cancellationToken)
        {
            SimpleResult result = new SimpleResult
            {
                Handlers = new List<Type> { GetType() }
            };
            return Task.FromResult(new PipelineAwareCommandHandlerResult<SimpleResult>(false, result));
        }
    }
}
