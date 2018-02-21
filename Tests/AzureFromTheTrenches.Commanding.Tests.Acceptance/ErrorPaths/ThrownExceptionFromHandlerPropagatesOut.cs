using System;
using AzureFromTheTrenches.Commanding.Tests.Acceptance.Helpers;
using Xbehave;
using Xunit;

namespace AzureFromTheTrenches.Commanding.Tests.Acceptance.ErrorPaths
{
    public class ThrownExceptionFromHandlerPropagatesOut : AbstractDispatchTestBase
    {
        public ThrownExceptionFromHandlerPropagatesOut() : base((registry, customDispatcher) =>
        {
            registry.Register<SimpleCommandWithIntegerResultHandler>();
        })
        {

        }

        [Scenario]
        public void ExecuteTest(SimpleCommandWithIntegerResult command, Exception thrownException)
        {
            "Given a command whose handler throws an exception"
                .x(() => command = new SimpleCommandWithIntegerResult(new Exception("something went wrong")));
            "When I dispatch the command"
                .x(async () =>
                {
                    try
                    {
                        await Dispatcher.DispatchAsync(command);
                    }
                    catch (Exception ex)
                    {
                        thrownException = ex;
                    }
                });
            "Then the exception is propagated back to the caller"
                .x(() =>
                {
                    Assert.NotNull(thrownException);
                    Assert.IsType<CommandExecutionException>(thrownException);
                    CommandExecutionException commandExecutionException = (CommandExecutionException) thrownException;
                    Assert.NotNull(commandExecutionException.InnerException);
                    Assert.Equal("something went wrong", commandExecutionException.InnerException.Message);
                });
        }
    }
}
