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
    public class CommandExecuterTests
    {
        [Fact]
        public async Task SimpleCommandExecutes()
        {
            // Arrange
            Mock<ICommandHandlerFactory> handlerFactory = new Mock<ICommandHandlerFactory>();
            Mock<ICommandRegistry> registry = new Mock<ICommandRegistry>();
            Mock<ICommandHandlerExecuter> commandHandlerExecuter = new Mock<ICommandHandlerExecuter>();
            Mock<IPipelineAwareCommandHandlerExecuter> commandHandlerChainExecuter = new Mock<IPipelineAwareCommandHandlerExecuter>();
            Mock<ICommandScopeManager> scopeManager = new Mock<ICommandScopeManager>();
            Mock<ICommandExecutionExceptionHandler> commandExecutionExceptionHandler = new Mock<ICommandExecutionExceptionHandler>();
            Mock<ICommandAuditPipeline> commandAuditPipeline = new Mock<ICommandAuditPipeline>();
            Mock<IOptionsProvider> optionsProvider = new Mock<IOptionsProvider>();
            optionsProvider.Setup(x => x.Options).Returns(new Options());
            handlerFactory.Setup(x => x.Create(typeof(SimpleCommandHandler))).Returns(new SimpleCommandHandler());
            registry.Setup(x => x.GetPrioritisedCommandHandlers(It.IsAny<ICommand>())).Returns(
                new List<PrioritisedCommandHandler>
                {
                    new PrioritisedCommandHandler(0, typeof(SimpleCommandHandler))
                });            

            CommandExecuter executer = new CommandExecuter(registry.Object,
                handlerFactory.Object,
                scopeManager.Object,
                commandHandlerExecuter.Object,
                commandHandlerChainExecuter.Object,
                commandExecutionExceptionHandler.Object,
                commandAuditPipeline.Object,
                optionsProvider.Object);
            SimpleCommand simpleCommand = new SimpleCommand();

            // Act
            await executer.ExecuteAsync(simpleCommand, default(CancellationToken));

            // Assert
            commandHandlerExecuter.Verify(x => x.ExecuteAsync(It.IsAny<ICommandHandler>(), simpleCommand, It.IsAny<SimpleResult>(), It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task MissingCommandActorsThrowsException()
        {
            // Arrange
            Mock<ICommandHandlerFactory> handlerFactory = new Mock<ICommandHandlerFactory>();
            Mock<ICommandRegistry> registry = new Mock<ICommandRegistry>();
            Mock<ICommandHandlerExecuter> commandHandlerExecuter = new Mock<ICommandHandlerExecuter>();
            Mock<IPipelineAwareCommandHandlerExecuter> commandHandlerChainExecuter = new Mock<IPipelineAwareCommandHandlerExecuter>();
            Mock<ICommandScopeManager> scopeManager = new Mock<ICommandScopeManager>();
            Mock<ICommandExecutionExceptionHandler> commandExecutionExceptionHandler = new Mock<ICommandExecutionExceptionHandler>();
            Mock<ICommandAuditPipeline> commandAuditPipeline = new Mock<ICommandAuditPipeline>();
            Mock<IOptionsProvider> optionsProvider = new Mock<IOptionsProvider>();
            optionsProvider.Setup(x => x.Options).Returns(new Options());
            handlerFactory.Setup(x => x.Create(typeof(SimpleCommandHandler))).Returns(new SimpleCommandHandler());
            registry.Setup(x => x.GetPrioritisedCommandHandlers(It.IsAny<ICommand>())).Returns<List<PrioritisedCommandHandler>>(null);

            CommandExecuter executer = new CommandExecuter(registry.Object,
                handlerFactory.Object,
                scopeManager.Object,
                commandHandlerExecuter.Object,
                commandHandlerChainExecuter.Object,
                commandExecutionExceptionHandler.Object,
                commandAuditPipeline.Object,
                optionsProvider.Object);

            // Act and assert
            MissingCommandHandlerRegistrationException ex = await Assert.ThrowsAsync<MissingCommandHandlerRegistrationException>(async () => await executer.ExecuteAsync(new SimpleCommand(), default(CancellationToken)));
            Assert.Equal(typeof(SimpleCommand), ex.CommandType);
        }

        [Fact]
        public async Task ErrorInHandlerInvokesRegisteredExceptionHandler()
        {
            // Arrange
            Mock<ICommandHandlerFactory> handlerFactory = new Mock<ICommandHandlerFactory>();
            Mock<ICommandRegistry> registry = new Mock<ICommandRegistry>();
            Mock<ICommandHandlerExecuter> commandHandlerExecuter = new Mock<ICommandHandlerExecuter>();
            Mock<IPipelineAwareCommandHandlerExecuter> commandHandlerChainExecuter = new Mock<IPipelineAwareCommandHandlerExecuter>();
            Mock<ICommandScopeManager> scopeManager = new Mock<ICommandScopeManager>();
            Mock<ICommandExecutionExceptionHandler> commandExecutionExceptionHandler = new Mock<ICommandExecutionExceptionHandler>();
            Mock<ICommandAuditPipeline> commandAuditPipeline = new Mock<ICommandAuditPipeline>();
            Mock<IOptionsProvider> optionsProvider = new Mock<IOptionsProvider>();
            optionsProvider.Setup(x => x.Options).Returns(new Options());
            handlerFactory.Setup(x => x.Create(typeof(SimpleCommandHandler))).Returns(new SimpleCommandHandler());
            registry.Setup(x => x.GetPrioritisedCommandHandlers(It.IsAny<ICommand>())).Returns(
                new List<PrioritisedCommandHandler>
                {
                    new PrioritisedCommandHandler(0, typeof(ThrowExceptionCommandHandler))
                });

            CommandExecuter executer = new CommandExecuter(registry.Object,
                handlerFactory.Object,
                scopeManager.Object,
                commandHandlerExecuter.Object,
                commandHandlerChainExecuter.Object,
                commandExecutionExceptionHandler.Object,
                commandAuditPipeline.Object,
                optionsProvider.Object);
            SimpleCommand simpleCommand = new SimpleCommand();

            // Act
            await executer.ExecuteAsync(simpleCommand, default(CancellationToken));

            // Assert
            commandExecutionExceptionHandler.Verify(x => x.HandleException(It.IsAny<Exception>(), It.IsAny<object>(), It.IsAny<int>(), simpleCommand, It.IsAny<ICommandDispatchContext>()));
        }

        [Fact]
        public async Task CommandChainHalts()
        {
            // Arrange
            Mock<ICommandHandlerFactory> handlerFactory = new Mock<ICommandHandlerFactory>();
            Mock<ICommandRegistry> registry = new Mock<ICommandRegistry>();
            Mock<ICommandHandlerExecuter> commandHandlerExecuter = new Mock<ICommandHandlerExecuter>();
            Mock<IPipelineAwareCommandHandlerExecuter> commandHandlerChainExecuter = new Mock<IPipelineAwareCommandHandlerExecuter>();
            Mock<ICommandScopeManager> scopeManager = new Mock<ICommandScopeManager>();
            Mock<ICommandExecutionExceptionHandler> commandExecutionExceptionHandler = new Mock<ICommandExecutionExceptionHandler>();
            Mock<ICommandAuditPipeline> commandAuditPipeline = new Mock<ICommandAuditPipeline>();
            Mock<IOptionsProvider> optionsProvider = new Mock<IOptionsProvider>();
            optionsProvider.Setup(x => x.Options).Returns(new Options());
            handlerFactory.Setup(x => x.Create(typeof(SimpleCommandHandler))).Returns(new SimpleCommandHandler());
            handlerFactory.Setup(x => x.Create(typeof(SimplePipelineAwareCommandHandlerThatHalts))).Returns(new SimplePipelineAwareCommandHandlerThatHalts());
            handlerFactory.Setup(x => x.Create(typeof(SimpleCommandHandlerTwo))).Returns(new SimpleCommandHandlerTwo());
            registry.Setup(x => x.GetPrioritisedCommandHandlers(It.IsAny<ICommand>())).Returns(
                new List<PrioritisedCommandHandler>
                {
                    new PrioritisedCommandHandler(0, typeof(SimpleCommandHandler)),
                    new PrioritisedCommandHandler(1, typeof(SimplePipelineAwareCommandHandlerThatHalts)),
                    new PrioritisedCommandHandler(2, typeof(SimpleCommandHandlerTwo))
                });
            SimpleCommand simpleCommand = new SimpleCommand();
            commandHandlerChainExecuter
                .Setup(x => x.ExecuteAsync(It.IsAny<IPipelineAwareCommandHandler>(), simpleCommand, It.IsAny<SimpleResult>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PipelineAwareCommandHandlerResult<SimpleResult>(true, null));
            
            CommandExecuter executer = new CommandExecuter(registry.Object,
                handlerFactory.Object,
                scopeManager.Object,
                commandHandlerExecuter.Object,
                commandHandlerChainExecuter.Object,
                commandExecutionExceptionHandler.Object,
                commandAuditPipeline.Object,
                optionsProvider.Object);

            // Act
            SimpleResult result = await executer.ExecuteAsync(simpleCommand, default(CancellationToken));

            // Assert
            // we should run the first SimpleCommandHandler as its not a chain command and won't be able to halt things and run the
            // SImpleCommandActorThatHalts once - it will halt and prevent the second non chained handler being called
            commandHandlerExecuter.Verify(x => x.ExecuteAsync(It.IsAny<ICommandHandler>(), simpleCommand, It.IsAny<SimpleResult>(), It.IsAny<CancellationToken>()), Times.Once);
            commandHandlerChainExecuter.Verify(x => x.ExecuteAsync(It.IsAny<IPipelineAwareCommandHandler>(), simpleCommand, It.IsAny<SimpleResult>(), It.IsAny<CancellationToken>()), Times.Exactly(1));
        }

        [Fact]
        public async Task CommandChain()
        {
            // Arrange
            Mock<ICommandHandlerFactory> handlerFactory = new Mock<ICommandHandlerFactory>();
            Mock<ICommandRegistry> registry = new Mock<ICommandRegistry>();
            Mock<ICommandHandlerExecuter> commandHandlerExecuter = new Mock<ICommandHandlerExecuter>();
            Mock<IPipelineAwareCommandHandlerExecuter> commandHandlerChainExecuter = new Mock<IPipelineAwareCommandHandlerExecuter>();
            Mock<ICommandScopeManager> scopeManager = new Mock<ICommandScopeManager>();
            Mock<ICommandExecutionExceptionHandler> commandExecutionExceptionHandler = new Mock<ICommandExecutionExceptionHandler>();
            Mock<ICommandAuditPipeline> commandAuditPipeline = new Mock<ICommandAuditPipeline>();
            Mock<IOptionsProvider> optionsProvider = new Mock<IOptionsProvider>();
            optionsProvider.Setup(x => x.Options).Returns(new Options());
            handlerFactory.Setup(x => x.Create(typeof(SimpleCommandHandler))).Returns(new SimpleCommandHandler());
            handlerFactory.Setup(x => x.Create(typeof(SimpleCommandHandlerTwo))).Returns(new SimpleCommandHandlerTwo());
            registry.Setup(x => x.GetPrioritisedCommandHandlers(It.IsAny<ICommand>())).Returns(
                new List<PrioritisedCommandHandler>
                {
                    new PrioritisedCommandHandler(0, typeof(SimpleCommandHandler)),
                    new PrioritisedCommandHandler(1, typeof(SimpleCommandHandlerTwo))
                });

            CommandExecuter executer = new CommandExecuter(registry.Object,
                handlerFactory.Object,
                scopeManager.Object,
                commandHandlerExecuter.Object,
                commandHandlerChainExecuter.Object,
                commandExecutionExceptionHandler.Object,
                commandAuditPipeline.Object,
                optionsProvider.Object);
            SimpleCommand simpleCommand = new SimpleCommand();

            // Act
            
            SimpleResult result = await executer.ExecuteAsync(simpleCommand, default(CancellationToken));

            // Assert
            // if the third command had run their would be two items in the list and .single would throw an exception
            commandHandlerExecuter.Verify(x => x.ExecuteAsync(It.IsAny<ICommandHandler>(), simpleCommand, It.IsAny<SimpleResult>(), It.IsAny<CancellationToken>()), Times.Exactly(2));           
        }

        [Fact]
        public async Task SuccessfulCommandExecutionCallsAuditor()
        {
            // Arrange
            Mock<ICommandHandlerFactory> handlerFactory = new Mock<ICommandHandlerFactory>();
            Mock<ICommandRegistry> registry = new Mock<ICommandRegistry>();
            Mock<ICommandHandlerExecuter> commandHandlerExecuter = new Mock<ICommandHandlerExecuter>();
            Mock<IPipelineAwareCommandHandlerExecuter> commandHandlerChainExecuter = new Mock<IPipelineAwareCommandHandlerExecuter>();
            Mock<ICommandScopeManager> scopeManager = new Mock<ICommandScopeManager>();
            Mock<ICommandExecutionExceptionHandler> commandExecutionExceptionHandler = new Mock<ICommandExecutionExceptionHandler>();
            Mock<ICommandAuditPipeline> commandAuditPipeline = new Mock<ICommandAuditPipeline>();
            Mock<IOptionsProvider> optionsProvider = new Mock<IOptionsProvider>();
            optionsProvider.Setup(x => x.Options).Returns(new Options());
            handlerFactory.Setup(x => x.Create(typeof(SimpleCommandHandler))).Returns(new SimpleCommandHandler());
            registry.Setup(x => x.GetPrioritisedCommandHandlers(It.IsAny<ICommand>())).Returns(
                new List<PrioritisedCommandHandler>
                {
                    new PrioritisedCommandHandler(0, typeof(SimpleCommandHandler))
                });

            CommandExecuter executer = new CommandExecuter(registry.Object,
                handlerFactory.Object,
                scopeManager.Object,
                commandHandlerExecuter.Object,
                commandHandlerChainExecuter.Object,
                commandExecutionExceptionHandler.Object,
                commandAuditPipeline.Object,
                optionsProvider.Object);
            SimpleCommand simpleCommand = new SimpleCommand();

            // Act
            await executer.ExecuteAsync(simpleCommand, default(CancellationToken));

            // Assert
            commandAuditPipeline.Verify(x => x.AuditExecution(simpleCommand, It.IsAny<ICommandDispatchContext>(), It.IsAny<long>(), true, It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task FailedCommandExecutionCallsAuditorAndRethrows()
        {
            // Arrange
            Mock<ICommandHandlerFactory> handlerFactory = new Mock<ICommandHandlerFactory>();
            Mock<ICommandRegistry> registry = new Mock<ICommandRegistry>();
            Mock<ICommandHandlerExecuter> commandHandlerExecuter = new Mock<ICommandHandlerExecuter>();
            Mock<IPipelineAwareCommandHandlerExecuter> commandHandlerChainExecuter = new Mock<IPipelineAwareCommandHandlerExecuter>();
            Mock<ICommandScopeManager> scopeManager = new Mock<ICommandScopeManager>();
            Mock<ICommandExecutionExceptionHandler> commandExecutionExceptionHandler = new Mock<ICommandExecutionExceptionHandler>();
            Mock<ICommandAuditPipeline> commandAuditPipeline = new Mock<ICommandAuditPipeline>();
            Mock<IOptionsProvider> optionsProvider = new Mock<IOptionsProvider>();
            optionsProvider.Setup(x => x.Options).Returns(new Options());
            handlerFactory.Setup(x => x.Create(typeof(SimpleCommandHandler))).Returns(new SimpleCommandHandler());
            registry.Setup(x => x.GetPrioritisedCommandHandlers(It.IsAny<ICommand>())).Returns(
                new List<PrioritisedCommandHandler>
                {
                    new PrioritisedCommandHandler(0, typeof(SimpleCommandHandler))
                });
            SimpleCommand simpleCommand = new SimpleCommand();
            commandHandlerExecuter.Setup(x => x.ExecuteAsync(It.IsAny<ICommandHandler>(),
                simpleCommand, It.IsAny<SimpleResult>(), It.IsAny<CancellationToken>())).Throws(new Exception());
            commandExecutionExceptionHandler.Setup(x => x.HandleException(It.IsAny<Exception>(),
                It.IsAny<object>(),
                It.IsAny<int>(),
                simpleCommand,
                It.IsAny<ICommandDispatchContext>())).Throws(new Exception());

            CommandExecuter executer = new CommandExecuter(registry.Object,
                handlerFactory.Object,
                scopeManager.Object,
                commandHandlerExecuter.Object,
                commandHandlerChainExecuter.Object,
                commandExecutionExceptionHandler.Object,
                commandAuditPipeline.Object,
                optionsProvider.Object);
            

            // Act
            await Assert.ThrowsAsync<Exception>(async () => await executer.ExecuteAsync(simpleCommand, default(CancellationToken)));

            // Assert
            commandAuditPipeline.Verify(x => x.AuditExecution(simpleCommand, It.IsAny<ICommandDispatchContext>(), It.IsAny<long>(), false, It.IsAny<CancellationToken>()));
        }
    }
}
