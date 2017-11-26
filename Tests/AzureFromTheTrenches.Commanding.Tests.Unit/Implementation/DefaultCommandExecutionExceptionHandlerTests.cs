using System;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Implementation;
using AzureFromTheTrenches.Commanding.Tests.Unit.TestModel;
using Moq;
using Xunit;

namespace AzureFromTheTrenches.Commanding.Tests.Unit.Implementation
{
    public class DefaultCommandExecutionExceptionHandlerTests
    {
        [Fact]
        public async Task RethrowsException()
        {
            // Arrange
            DefaultCommandExecutionExceptionHandler testSubject = new DefaultCommandExecutionExceptionHandler();

            // Act
            await Assert.ThrowsAsync<CommandExecutionException>(async () =>
                await testSubject.HandleException(new InvalidOperationException(), new object(), 1, new SimpleCommand(),
                    new Mock<ICommandDispatchContext>().Object));

        }
    }
}
