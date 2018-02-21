using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions.Model;
using AzureFromTheTrenches.Commanding.Implementation;
using AzureFromTheTrenches.Commanding.Tests.Unit.TestModel;
using Xunit;

namespace AzureFromTheTrenches.Commanding.Tests.Unit.Implementation
{
    public class CommandHandlerExecuterTests
    {
        [Fact]
        public async Task ExecutesHandlerWithResult()
        {
            // Arrange
            CommandHandlerExecuter testSubject = new CommandHandlerExecuter();
            SimpleCommandHandler handler = new SimpleCommandHandler();
            SimpleCommand command = new SimpleCommand();
            testSubject.CompileHandlerExecuter(typeof(SimpleCommand), typeof(SimpleCommandHandler));

            // Act
            SimpleResult result = await testSubject.ExecuteAsync(handler, command, null, CancellationToken.None);

            // Assert
            Assert.Single(result.Handlers);
            Assert.Equal(typeof(SimpleCommandHandler), result.Handlers.Single());
        }

        [Fact]
        public async Task ExecutesCancellableHandlerWithResult()
        {
            // Arrange
            CommandHandlerExecuter testSubject = new CommandHandlerExecuter();
            CancellableSimpleCommandHandler handler = new CancellableSimpleCommandHandler();
            SimpleCommand command = new SimpleCommand();
            testSubject.CompileHandlerExecuter(typeof(SimpleCommand), typeof(CancellableSimpleCommandHandler));

            // Act
            SimpleResult result = await testSubject.ExecuteAsync(handler, command, null, CancellationToken.None);

            // Assert
            Assert.Single(result.Handlers);
            Assert.Equal(typeof(CancellableSimpleCommandHandler), result.Handlers.Single());
        }

        [Fact]
        public async Task ExecutesHandlerWithNoResult()
        {
            // Arrange
            CommandHandlerExecuter testSubject = new CommandHandlerExecuter();
            SimpleCommandNoResultHandler handler = new SimpleCommandNoResultHandler();
            SimpleCommandNoResult command = new SimpleCommandNoResult();
            testSubject.CompileHandlerExecuter(typeof(SimpleCommandNoResult), typeof(SimpleCommandNoResultHandler));

            // Act
            await testSubject.ExecuteAsync(handler, new NoResultCommandWrapper(command), null, CancellationToken.None);

            // Assert
            Assert.True(command.WasHandled);
        }

        [Fact]
        public async Task ExecutesCancellableHandlerWithNoResult()
        {
            // Arrange
            CommandHandlerExecuter testSubject = new CommandHandlerExecuter();
            CancellableSimpleCommandNoResultHandler handler = new CancellableSimpleCommandNoResultHandler();
            SimpleCommandNoResult command = new SimpleCommandNoResult();
            testSubject.CompileHandlerExecuter(typeof(SimpleCommandNoResult), typeof(CancellableSimpleCommandNoResultHandler));

            // Act
            await testSubject.ExecuteAsync(handler, new NoResultCommandWrapper(command), null, CancellationToken.None);

            // Assert
            Assert.True(command.WasHandled);
        }
    }
}
