using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Tests.Acceptance.Helpers;
using Xbehave;
using Xunit;

namespace AzureFromTheTrenches.Commanding.Tests.Acceptance
{
    public class SimpleCommandWithIntegerResultTest : AbstractDispatchTestBase
    {
        public SimpleCommandWithIntegerResultTest() : base((registry) =>
        {
            registry.Register<SimpleCommandWithIntegerResultHandler>();
        })
        {

        }

        /*[Scenario]
        public void ExecuteTest(SimpleCommandWithIntegerResult command, int result)
        {
            "Given a command with no result"
                .x(() => command = new SimpleCommandWithIntegerResult());
            "When I dispatch the command"
                .x(async () => result = await Dispatcher.DispatchAsync(command));
            "Then the associated command handler is executed"
                .x(() =>
                {
                    Assert.Equal(99, result);
                }); 
        }*/

        [Fact]
        public async Task NonAcceptanceApproach()
        {
            SimpleCommandWithIntegerResult command = new SimpleCommandWithIntegerResult();
            int result = await Dispatcher.DispatchAsync(command); 
            Assert.Equal(99, result);
        }
    }
}
