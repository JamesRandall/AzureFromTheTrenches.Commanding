using System.Threading.Tasks;
using AccidentalFish.Commanding.Model;

namespace AccidentalFish.Commanding.Tests.Unit.TestModel
{
    class SimpleCommandActorThatHalts : ICommandChainActor<SimpleCommand, SimpleResult>
    {
        public Task<CommandChainActorResult<SimpleResult>> ExecuteAsync(SimpleCommand command, SimpleResult previousResult)
        {
            return Task.FromResult(new CommandChainActorResult<SimpleResult>(true, previousResult));
        }
    }
}
