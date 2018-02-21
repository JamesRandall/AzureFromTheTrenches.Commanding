using AzureFromTheTrenches.Commanding.Tests.Acceptance.Helpers;
using Xbehave;
using Xunit;

namespace AzureFromTheTrenches.Commanding.Tests.Acceptance.HappyPaths
{
    public class CustomExecuterForCommandWithResultTest : AbstractDispatchTestBase
    {
        public CustomExecuterForCommandWithResultTest() : base((registry, customDispatcher) =>
        {
            registry.Register<SimpleCommandWithIntegerResult>(() => customDispatcher);
        })
        {
            CustomDispatcher.AssociatedExecuter = CustomExecuter;
        }

        [Scenario]
        public void ExecuteTest(SimpleCommandWithIntegerResult command)
        {
            "Given a command that returns a result"
                .x(() => command = new SimpleCommandWithIntegerResult());
            "When I dispatch the command using a custom dispatcher"
                .x(async () => await Dispatcher.DispatchAsync(command));
            "Then the custom dispatcher is used"
                .x(() =>
                {
                    Assert.Equal(1, CustomExecuter.Log.Count);
                    Assert.Contains("Executing command of type SimpleCommandWithIntegerResult with custom executer", CustomExecuter.Log);
                });
        }
    }
}
