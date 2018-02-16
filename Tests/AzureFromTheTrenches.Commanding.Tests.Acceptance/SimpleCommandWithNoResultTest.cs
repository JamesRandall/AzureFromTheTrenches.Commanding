using AzureFromTheTrenches.Commanding.Tests.Acceptance.Helpers;
using Xbehave;
using Xunit;

namespace AzureFromTheTrenches.Commanding.Tests.Acceptance
{
    public class SimpleCommandWithNoResultTest : AbstractDispatchTestBase
    {
        public SimpleCommandWithNoResultTest() : base(registry => { registry.Register<SimpleCommandWithNoResultHandler>(); })
        {
        }

        [Scenario]
        public void ExecuteTest(SimpleCommandWithNoResult command)
        {
            "Given a command with no result"
                .x(() => command = new SimpleCommandWithNoResult());
            "When I dispatch the command"
                .x(async () => await Dispatcher.DispatchAsync(command));
            "Then the associated command handler is executed"
                .x(() => 
                {
                    Assert.Equal(1, CommandTracer.LoggedItems.Count);
                    Assert.Contains("Executed SimpleCommandWithNoResultHandler", CommandTracer.LoggedItems);
                });
        }
    }
}
