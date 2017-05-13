using System.Threading.Tasks;
using AccidentalFish.Commanding.Implementation;
using AccidentalFish.Commanding.Model;
using AccidentalFish.Commanding.Tests.Unit.TestModel;
using Moq;
using Xunit;

namespace AccidentalFish.Commanding.Tests.Unit.Implementation
{
    public class CommandDispatcherTests
    {
        [Fact]
        public async Task ExecutesWithoutDispatcher()
        {
            // Arrange
            Mock<ICommandRegistry> registry = new Mock<ICommandRegistry>();
            Mock<ICommandExecuter> executer = new Mock<ICommandExecuter>();
            CommandDispatcher dispatcher = new CommandDispatcher(registry.Object, executer.Object);
            SimpleCommand command = new SimpleCommand();

            // Act
            await dispatcher.DispatchAsync<SimpleCommand, SimpleResult>(command);

            // Assert
            executer.Verify(x => x.ExecuteAsync<SimpleCommand, SimpleResult>(command));
        }

        [Fact]
        public async Task DispatcherDefersExecution()
        {
            // Arrange
            Mock<ICommandRegistry> registry = new Mock<ICommandRegistry>();
            Mock<ICommandExecuter> executer = new Mock<ICommandExecuter>();
            Mock<ICommandDispatcher> commandDispatcher = new Mock<ICommandDispatcher>();
            CommandDispatcher dispatcher = new CommandDispatcher(registry.Object, executer.Object);
            SimpleCommand command = new SimpleCommand();
            registry.Setup(x => x.GetCommandDispatcherFactory<SimpleCommand>()).Returns(() => commandDispatcher.Object);
            commandDispatcher.Setup(x => x.DispatchAsync<SimpleCommand, SimpleResult>(command)).ReturnsAsync(new CommandResult<SimpleResult>(null, true));

            // Act
            await dispatcher.DispatchAsync<SimpleCommand, SimpleResult>(command);

            // Assert
            executer.Verify(x => x.ExecuteAsync<SimpleCommand, SimpleResult>(command), Times.Never);
        }

        [Fact]
        public async Task DispatcherUsesAssociatedExecuter()
        {
            // Arrange
            Mock<ICommandRegistry> registry = new Mock<ICommandRegistry>();
            Mock<ICommandExecuter> executer = new Mock<ICommandExecuter>();
            Mock<ICommandDispatcher> commandDispatcher = new Mock<ICommandDispatcher>();
            Mock<ICommandExecuter> associatedExecuter = new Mock<ICommandExecuter>();
            CommandDispatcher dispatcher = new CommandDispatcher(registry.Object, executer.Object);
            SimpleCommand command = new SimpleCommand();
            registry.Setup(x => x.GetCommandDispatcherFactory<SimpleCommand>()).Returns(() => commandDispatcher.Object);
            commandDispatcher.Setup(x => x.DispatchAsync<SimpleCommand, SimpleResult>(command)).ReturnsAsync(new CommandResult<SimpleResult>(null, false));
            commandDispatcher.SetupGet(x => x.AssociatedExecuter).Returns(associatedExecuter.Object);

            // Act
            await dispatcher.DispatchAsync<SimpleCommand, SimpleResult>(command);

            // Assert
            executer.Verify(x => x.ExecuteAsync<SimpleCommand, SimpleResult>(command), Times.Never);
            associatedExecuter.Verify(x => x.ExecuteAsync<SimpleCommand, SimpleResult>(command), Times.Once);
        }
    }
}
