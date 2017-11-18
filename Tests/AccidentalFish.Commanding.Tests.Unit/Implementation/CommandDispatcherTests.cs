using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AccidentalFish.Commanding.Abstractions;
using AccidentalFish.Commanding.Abstractions.Model;
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
            Mock<ICommandScopeManager> commandContextManager = new Mock<ICommandScopeManager>();
            Mock<ICommandDispatcherOptions> options = new Mock<ICommandDispatcherOptions>();            
            Mock<ICommandAuditPipeline> auditorPipeline = new Mock<ICommandAuditPipeline>();
            CommandDispatcher dispatcher = new CommandDispatcher(registry.Object, executer.Object, commandContextManager.Object, auditorPipeline.Object, options.Object);
            SimpleCommand command = new SimpleCommand();

            // Act
            await dispatcher.DispatchAsync(command);

            // Assert
            executer.Verify(x => x.ExecuteAsync(command));
        }

        [Fact]
        public async Task DispatcherDefersExecution()
        {
            // Arrange
            Mock<ICommandRegistry> registry = new Mock<ICommandRegistry>();
            Mock<ICommandExecuter> executer = new Mock<ICommandExecuter>();
            Mock<ICommandScopeManager> commandContextManager = new Mock<ICommandScopeManager>();
            Mock<ICommandAuditPipeline> auditorPipeline = new Mock<ICommandAuditPipeline>();
            Mock<ICommandDispatcherOptions> options = new Mock<ICommandDispatcherOptions>();
            Mock<ICommandDispatcher> commandDispatcher = new Mock<ICommandDispatcher>();
            CommandDispatcher dispatcher = new CommandDispatcher(registry.Object, executer.Object,commandContextManager.Object, auditorPipeline.Object, options.Object);
            SimpleCommand command = new SimpleCommand();
            registry.Setup(x => x.GetCommandDispatcherFactory(command)).Returns(() => commandDispatcher.Object);
            commandDispatcher.Setup(x => x.DispatchAsync(command)).ReturnsAsync(new CommandResult<SimpleResult>(null, true));

            // Act
            await dispatcher.DispatchAsync(command);

            // Assert
            executer.Verify(x => x.ExecuteAsync(command), Times.Never);
        }

        [Fact]
        public async Task DispatcherUsesAssociatedExecuter()
        {
            // Arrange
            Mock<ICommandRegistry> registry = new Mock<ICommandRegistry>();
            Mock<ICommandExecuter> executer = new Mock<ICommandExecuter>();
            Mock<ICommandScopeManager> commandContextManager = new Mock<ICommandScopeManager>();
            Mock<ICommandAuditPipeline> auditorPipeline = new Mock<ICommandAuditPipeline>();
            Mock<ICommandDispatcherOptions> options = new Mock<ICommandDispatcherOptions>();
            Mock<ICommandDispatcher> commandDispatcher = new Mock<ICommandDispatcher>();
            Mock<ICommandExecuter> associatedExecuter = new Mock<ICommandExecuter>();
            CommandDispatcher dispatcher = new CommandDispatcher(registry.Object, executer.Object, commandContextManager.Object, auditorPipeline.Object, options.Object);
            SimpleCommand command = new SimpleCommand();
            registry.Setup(x => x.GetCommandDispatcherFactory(command)).Returns(() => commandDispatcher.Object);
            commandDispatcher.Setup(x => x.DispatchAsync(command)).ReturnsAsync(new CommandResult<SimpleResult>(null, false));
            commandDispatcher.SetupGet(x => x.AssociatedExecuter).Returns(associatedExecuter.Object);

            // Act
            await dispatcher.DispatchAsync(command);

            // Assert
            executer.Verify(x => x.ExecuteAsync(command), Times.Never);
            associatedExecuter.Verify(x => x.ExecuteAsync(command), Times.Once);
        }

        [Fact]
        public async Task EntersScope()
        {
            // Arrange
            Mock<ICommandRegistry> registry = new Mock<ICommandRegistry>();
            Mock<ICommandExecuter> executer = new Mock<ICommandExecuter>();
            Mock<ICommandScopeManager> commandContextManager = new Mock<ICommandScopeManager>();
            Mock<ICommandAuditPipeline> auditorPipeline = new Mock<ICommandAuditPipeline>();
            Mock<ICommandDispatcherOptions> options = new Mock<ICommandDispatcherOptions>();
            CommandDispatcher dispatcher = new CommandDispatcher(registry.Object, executer.Object, commandContextManager.Object, auditorPipeline.Object, options.Object);
            SimpleCommand command = new SimpleCommand();

            // Act
            await dispatcher.DispatchAsync(command);

            // Assert
            commandContextManager.Verify(x => x.Enter(), Times.Once);
        }

        [Fact]
        public async Task ExitsScope()
        {
            // Arrange
            Mock<ICommandRegistry> registry = new Mock<ICommandRegistry>();
            Mock<ICommandExecuter> executer = new Mock<ICommandExecuter>();
            Mock<ICommandScopeManager> commandContextManager = new Mock<ICommandScopeManager>();
            Mock<ICommandAuditPipeline> auditorPipeline = new Mock<ICommandAuditPipeline>();
            Mock<ICommandDispatcherOptions> options = new Mock<ICommandDispatcherOptions>();
            CommandDispatcher dispatcher = new CommandDispatcher(registry.Object, executer.Object, commandContextManager.Object, auditorPipeline.Object, options.Object);
            SimpleCommand command = new SimpleCommand();

            // Act
            await dispatcher.DispatchAsync(command);

            // Assert
            commandContextManager.Verify(x => x.Exit(), Times.Once);
        }

        [Fact]
        public async Task ExitsScopeOnException()
        {
            // Arrange
            Mock<ICommandRegistry> registry = new Mock<ICommandRegistry>();
            Mock<ICommandExecuter> executer = new Mock<ICommandExecuter>();
            Mock<ICommandScopeManager> commandContextManager = new Mock<ICommandScopeManager>();
            Mock<ICommandAuditPipeline> auditorPipeline = new Mock<ICommandAuditPipeline>();
            Mock<ICommandDispatcherOptions> options = new Mock<ICommandDispatcherOptions>();
            CommandDispatcher dispatcher = new CommandDispatcher(registry.Object, executer.Object, commandContextManager.Object, auditorPipeline.Object, options.Object);
            SimpleCommand command = new SimpleCommand();
            executer.Setup(x => x.ExecuteAsync(command)).Throws(new InvalidOperationException());

            // Act
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await dispatcher.DispatchAsync(command));

            // Assert
            commandContextManager.Verify(x => x.Exit(), Times.Once);
        }

        [Fact]
        public async Task AuditsBeforeExecute()
        {
            // Arrange
            int executionOrder = 0;
            int auditExecutionIndex = -1;
            int executeExecutionIndex = -1;
            Mock<ICommandRegistry> registry = new Mock<ICommandRegistry>();
            Mock<ICommandExecuter> executer = new Mock<ICommandExecuter>();
            Mock<ICommandScopeManager> commandContextManager = new Mock<ICommandScopeManager>();
            Mock<ICommandAuditPipeline> auditorPipeline = new Mock<ICommandAuditPipeline>();
            Mock<ICommandDispatcherOptions> options = new Mock<ICommandDispatcherOptions>();
            CommandDispatcher dispatcher = new CommandDispatcher(registry.Object, executer.Object, commandContextManager.Object, auditorPipeline.Object, options.Object);
            CommandDispatchContext commandDispatchContext = new CommandDispatchContext("someid", new Dictionary<string, object>());
            commandContextManager.Setup(x => x.Enter()).Returns(commandDispatchContext);
            SimpleCommand command = new SimpleCommand();
            auditorPipeline.Setup(x => x.Audit(command, It.IsAny<Guid>(), commandDispatchContext)).Callback(() =>
            {
                auditExecutionIndex = executionOrder;
                executionOrder++;
            }).Returns(Task.FromResult(0));
            executer.Setup(x => x.ExecuteAsync(command)).Callback(() =>
            {
                executeExecutionIndex = executionOrder;
                executionOrder++;
            }).Returns(Task.FromResult<SimpleResult>(null));

            // Act
            await dispatcher.DispatchAsync(command);

            // Assert
            auditorPipeline.Verify(x => x.Audit(command, It.IsAny<Guid>(), commandDispatchContext), Times.Once);
            Assert.Equal(0, auditExecutionIndex);
            Assert.Equal(1, executeExecutionIndex);
        }
    }
}
