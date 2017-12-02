using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;

namespace AzureFromTheTrenches.Commanding.Tests.Unit.TestModel
{
    class SimpleCommandPipelineAwareHandler : IPipelineAwareCommandHandler<SimpleCommand, SimpleResult>
    {
        public Task<PipelineAwareCommandHandlerResult<SimpleResult>> ExecuteAsync(SimpleCommand command, SimpleResult previousResult)
        {
            SimpleResult result = new SimpleResult
            {
                Handlers = new List<Type> { GetType() }
            };
            return Task.FromResult(new PipelineAwareCommandHandlerResult<SimpleResult>(false, result));
        }
    }
}
