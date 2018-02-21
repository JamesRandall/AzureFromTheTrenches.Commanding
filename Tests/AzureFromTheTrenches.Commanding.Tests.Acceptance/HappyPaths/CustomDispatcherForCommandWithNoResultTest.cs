using System;
using System.Collections.Generic;
using System.Text;
using AzureFromTheTrenches.Commanding.Tests.Acceptance.Helpers;
using Xbehave;
using Xunit;

namespace AzureFromTheTrenches.Commanding.Tests.Acceptance.HappyPaths
{
    public class CustomDispatcherForCommandWithNoResultTest : AbstractDispatchTestBase
    {
        public CustomDispatcherForCommandWithNoResultTest() : base((registry, customDispatcher) =>
        {
            registry.Register<SimpleCommandWithNoResult>(() => customDispatcher);
        })
        {
        }

        [Scenario]
        public void ExecuteTest(SimpleCommandWithNoResult command)
        {
            "Given a command with no result"
                .x(() => command = new SimpleCommandWithNoResult());
            "When I dispatch the command using a custom dispatcher"
                .x(async () => await Dispatcher.DispatchAsync(command));
            "Then the custom dispatcher is used"
                .x(() =>
                {
                    Assert.Equal(1, CustomDispatcher.Log.Count);
                    Assert.Contains("Command of type SimpleCommandWithNoResult dispatched", CustomDispatcher.Log);
                });
        }
    }
}
