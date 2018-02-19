using System;
using AzureFromTheTrenches.Commanding.Tests.Acceptance.Helpers;
using Xbehave;
using Xunit;

namespace AzureFromTheTrenches.Commanding.Tests.Acceptance.ErrorPaths
{
    public class UnregisteredHandlerThrowsException : AbstractDispatchTestBase
    {
        public UnregisteredHandlerThrowsException() : base(r => {})
        {
        }

        [Scenario]
        public void ExecuteTest(SimpleCommandWithIntegerResult command, Exception ex)
        {
            "Given a command with no registered handler"
                .x(() => command = new SimpleCommandWithIntegerResult());
            "When I dispatch the command"
                .x(async () =>
                {
                    try
                    {
                        await Dispatcher.DispatchAsync(command);
                    }
                    catch (Exception e)
                    {
                        ex = e;
                    }
                    
                });
            "Then a meaningful exception is raised"
                .x(() =>
                {
                    Assert.NotNull(ex);
                    Assert.IsType<MissingCommandHandlerRegistrationException>(ex);
                });
        }
    }
}
