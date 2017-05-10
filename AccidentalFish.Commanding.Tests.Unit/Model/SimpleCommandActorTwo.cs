using System.Threading.Tasks;

namespace AccidentalFish.Commanding.Tests.Unit.Model
{
    class SimpleCommandActorTwo : ICommandActor<SimpleCommand>
    {
        public Task ExecuteAsync(SimpleCommand command)
        {
            return Task.FromResult(0);
        }
    }
}
