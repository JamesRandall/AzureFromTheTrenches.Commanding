using AzureFromTheTrenches.Commanding.Tests.Acceptance.Helpers;
using Xbehave;
using Xunit;

namespace AzureFromTheTrenches.Commanding.Tests.Acceptance.HappyPaths
{
    public class SimpleCommandWithIntegerResultTest : AbstractDispatchTestBase
    {
        public SimpleCommandWithIntegerResultTest() : base((registry) =>
        {
            registry.Register<SimpleCommandWithIntegerResultHandler>();
        })
        {

        }

        [Scenario]
        public void ExecuteTest(SimpleCommandWithIntegerResult command, int result)
        {
            "Given a command with an integer result"
                .x(() => command = new SimpleCommandWithIntegerResult());
            "When I dispatch the command"
                .x(async () => result = await Dispatcher.DispatchAsync(command));
            "Then the associated command handler is executed and the correct result returned"
                .x(() =>
                {
                    Assert.Equal(99, result);
                }); 
        }
    }
}
