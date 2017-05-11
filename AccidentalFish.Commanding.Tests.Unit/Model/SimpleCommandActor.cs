using System.Threading.Tasks;

namespace AccidentalFish.Commanding.Tests.Unit.Model
{
    internal class SimpleCommandActor : ICommandActor<SimpleCommand, SimpleResult>
    {
        public Task<SimpleResult> ExecuteAsync(SimpleCommand command, SimpleResult previousResult)
        {
            return Task.FromResult(new SimpleResult());
        }
    }
}
