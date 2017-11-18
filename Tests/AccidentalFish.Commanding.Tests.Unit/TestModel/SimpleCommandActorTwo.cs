using System.Threading.Tasks;
using AccidentalFish.Commanding.Abstractions;

namespace AccidentalFish.Commanding.Tests.Unit.TestModel
{
    class SimpleCommandActorTwo : ICommandActor<SimpleCommand, SimpleResult>
    {
        public Task<SimpleResult> ExecuteAsync(SimpleCommand command, SimpleResult previousResult)
        {
            previousResult.Actors.Add(GetType());
            return Task.FromResult(previousResult);
        }
    }
}
