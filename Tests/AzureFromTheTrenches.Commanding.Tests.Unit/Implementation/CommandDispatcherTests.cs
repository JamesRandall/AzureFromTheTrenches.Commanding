using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;
using AzureFromTheTrenches.Commanding.Implementation;
using AzureFromTheTrenches.Commanding.Model;
using AzureFromTheTrenches.Commanding.Tests.Unit.TestModel;
using Moq;
using Xunit;

namespace AzureFromTheTrenches.Commanding.Tests.Unit.Implementation
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
            Mock<ICommandAuditPipeline> auditorPipeline = new Mock<ICommandAuditPipeline>();
            Mock<IOptionsProvider> optionsProvider = new Mock<IOptionsProvider>();
            optionsProvider.Setup(x => x.Options).Returns(new Options());
            CommandDispatcher dispatcher = new CommandDispatcher(registry.Object,
                executer.Object,
                commandContextManager.Object,
                auditorPipeline.Object,
                optionsProvider.Object);
            SimpleCommand command = new SimpleCommand();

            // Act
            await dispatcher.DispatchAsync(command, default(CancellationToken));

            // Assert
            executer.Verify(x => x.ExecuteAsync(command, It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task DispatcherDefersExecution()
        {
            // Arrange
            Mock<ICommandRegistry> registry = new Mock<ICommandRegistry>();
            Mock<ICommandExecuter> executer = new Mock<ICommandExecuter>();
            Mock<ICommandScopeManager> commandContextManager = new Mock<ICommandScopeManager>();
            Mock<ICommandAuditPipeline> auditorPipeline = new Mock<ICommandAuditPipeline>();
            Mock<ICommandDispatcher> commandDispatcher = new Mock<ICommandDispatcher>();
            Mock<IOptionsProvider> optionsProvider = new Mock<IOptionsProvider>();
            optionsProvider.Setup(x => x.Options).Returns(new Options());
            CommandDispatcher dispatcher = new CommandDispatcher(
                registry.Object,
                executer.Object,
                commandContextManager.Object,
                auditorPipeline.Object,
                optionsProvider.Object);
            SimpleCommand command = new SimpleCommand();
            registry.Setup(x => x.GetCommandDispatcherFactory(command)).Returns(() => commandDispatcher.Object);
            commandDispatcher.Setup(x => x.DispatchAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(new CommandResult<SimpleResult>(null, true));

            // Act
            await dispatcher.DispatchAsync(command, default(CancellationToken));

            // Assert
            executer.Verify(x => x.ExecuteAsync(command, It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DispatcherUsesAssociatedExecuter()
        {
            // Arrange
            Mock<ICommandRegistry> registry = new Mock<ICommandRegistry>();
            Mock<ICommandExecuter> executer = new Mock<ICommandExecuter>();
            Mock<ICommandScopeManager> commandContextManager = new Mock<ICommandScopeManager>();
            Mock<ICommandAuditPipeline> auditorPipeline = new Mock<ICommandAuditPipeline>();
            Mock<ICommandDispatcher> commandDispatcher = new Mock<ICommandDispatcher>();
            Mock<ICommandExecuter> associatedExecuter = new Mock<ICommandExecuter>();
            Mock<IOptionsProvider> optionsProvider = new Mock<IOptionsProvider>();
            optionsProvider.Setup(x => x.Options).Returns(new Options());
            CommandDispatcher dispatcher = new CommandDispatcher(
                registry.Object,
                executer.Object,
                commandContextManager.Object,
                auditorPipeline.Object,
                optionsProvider.Object);
            SimpleCommand command = new SimpleCommand();
            registry.Setup(x => x.GetCommandDispatcherFactory(command)).Returns(() => commandDispatcher.Object);
            commandDispatcher.Setup(x => x.DispatchAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(new CommandResult<SimpleResult>(null, false));
            commandDispatcher.SetupGet(x => x.AssociatedExecuter).Returns(associatedExecuter.Object);

            // Act
            await dispatcher.DispatchAsync(command, default(CancellationToken));

            // Assert
            executer.Verify(x => x.ExecuteAsync(command, It.IsAny<CancellationToken>()), Times.Never);
            associatedExecuter.Verify(x => x.ExecuteAsync(command, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task EntersScope()
        {
            // Arrange
            Mock<ICommandRegistry> registry = new Mock<ICommandRegistry>();
            Mock<ICommandExecuter> executer = new Mock<ICommandExecuter>();
            Mock<ICommandScopeManager> commandContextManager = new Mock<ICommandScopeManager>();
            Mock<ICommandAuditPipeline> auditorPipeline = new Mock<ICommandAuditPipeline>();
            Mock<IOptionsProvider> optionsProvider = new Mock<IOptionsProvider>();
            optionsProvider.Setup(x => x.Options).Returns(new Options());
            CommandDispatcher dispatcher = new CommandDispatcher(
                registry.Object,
                executer.Object,
                commandContextManager.Object,
                auditorPipeline.Object,
                optionsProvider.Object);
            SimpleCommand command = new SimpleCommand();

            // Act
            await dispatcher.DispatchAsync(command, default(CancellationToken));

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
            Mock<IOptionsProvider> optionsProvider = new Mock<IOptionsProvider>();
            optionsProvider.Setup(x => x.Options).Returns(new Options());
            CommandDispatcher dispatcher = new CommandDispatcher(
                registry.Object,
                executer.Object,
                commandContextManager.Object,
                auditorPipeline.Object,
                optionsProvider.Object);
            SimpleCommand command = new SimpleCommand();

            // Act
            await dispatcher.DispatchAsync(command, default(CancellationToken));

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
            Mock<IOptionsProvider> optionsProvider = new Mock<IOptionsProvider>();
            optionsProvider.Setup(x => x.Options).Returns(new Options());
            CommandDispatcher dispatcher = new CommandDispatcher(
                registry.Object,
                executer.Object,
                commandContextManager.Object,
                auditorPipeline.Object,
                optionsProvider.Object);
            SimpleCommand command = new SimpleCommand();
            executer.Setup(x => x.ExecuteAsync(command, It.IsAny<CancellationToken>())).Throws(new InvalidOperationException());

            // Act
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await dispatcher.DispatchAsync(command, default(CancellationToken)));

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
            Mock<IOptionsProvider> optionsProvider = new Mock<IOptionsProvider>();
            optionsProvider.Setup(x => x.Options).Returns(new Options());
            CommandDispatcher dispatcher = new CommandDispatcher(
                registry.Object,
                executer.Object,
                commandContextManager.Object,
                auditorPipeline.Object,
                optionsProvider.Object);
            CommandDispatchContext commandDispatchContext = new CommandDispatchContext("someid", new Dictionary<string, object>());
            commandContextManager.Setup(x => x.Enter()).Returns(commandDispatchContext);
            SimpleCommand command = new SimpleCommand();
            auditorPipeline.Setup(x => x.AuditPreDispatch(command, commandDispatchContext, It.IsAny<CancellationToken>())).Callback(() =>
            {
                auditExecutionIndex = executionOrder;
                executionOrder++;
            }).Returns(Task.FromResult(0));
            executer.Setup(x => x.ExecuteAsync(command, It.IsAny<CancellationToken>())).Callback(() =>
            {
                executeExecutionIndex = executionOrder;
                executionOrder++;
            }).Returns(Task.FromResult<SimpleResult>(null));

            // Act
            await dispatcher.DispatchAsync(command, default(CancellationToken));

            // Assert
            auditorPipeline.Verify(x => x.AuditPreDispatch(command, commandDispatchContext, It.IsAny<CancellationToken>()), Times.Once);
            Assert.Equal(0, auditExecutionIndex);
            Assert.Equal(1, executeExecutionIndex);
        }
    }
}
