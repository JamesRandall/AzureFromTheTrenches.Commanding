using AzureFromTheTrenches.Commanding.Tests.Acceptance.Helpers;
using Xbehave;
using Xunit;

namespace AzureFromTheTrenches.Commanding.Tests.Acceptance.HappyPaths
{
    public class CustomExecuterForCommandWithNoResultTest : AbstractDispatchTestBase
    {
        public CustomExecuterForCommandWithNoResultTest() : base((registry, customDispatcher) =>
        {
            registry.Register<SimpleCommandWithNoResult>(() => customDispatcher);
        })
        {
            CustomDispatcher.AssociatedExecuter = CustomExecuter;
        }

        [Scenario]
        public void ExecuteTest(SimpleCommandWithNoResult command)
        {
            "Given a command that returns a result"
                .x(() => command = new SimpleCommandWithNoResult());
            "When I dispatch the command using a custom dispatcher"
                .x(async () => await Dispatcher.DispatchAsync(command));
            "Then the custom dispatcher is used"
                .x(() =>
                {
                    Assert.Equal(1, CustomExecuter.Log.Count);
                    Assert.Contains("Executing command of type SimpleCommandWithNoResult with custom executer", CustomExecuter.Log);
                });
        }
    }
}
