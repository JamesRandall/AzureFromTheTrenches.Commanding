using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AccidentalFish.Commanding.Abstractions;

namespace AccidentalFish.Commanding.Tests.Unit.TestModel
{
    internal class SimpleCommandActor : ICommandActor<SimpleCommand, SimpleResult>
    {
        public Task<SimpleResult> ExecuteAsync(SimpleCommand command, SimpleResult previousResult)
        {
            SimpleResult result = new SimpleResult
            {
                Actors = new List<Type> {GetType()}
            };
            return Task.FromResult(result);
        }
    }
}
