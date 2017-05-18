using System.Threading.Tasks;

namespace AccidentalFish.Commanding.Tests.Performance.Console.Model
{
    class SimpleActor : ICommandActor<SimpleCommand, SimpleResult>
    {
        public Task<SimpleResult> ExecuteAsync(SimpleCommand command, SimpleResult previousResult)
        {
            return Task.FromResult(previousResult);
        }
    }
}
