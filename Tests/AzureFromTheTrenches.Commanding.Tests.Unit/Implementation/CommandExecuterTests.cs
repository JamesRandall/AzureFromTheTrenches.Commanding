using System.Collections.Generic;
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
            Mock<ICommandHandlerChainExecuter> commandHandlerChainExecuter = new Mock<ICommandHandlerChainExecuter>();
            Mock<ICommandScopeManager> scopeManager = new Mock<ICommandScopeManager>();
            handlerFactory.Setup(x => x.Create(typeof(SimpleCommandHandler))).Returns(new SimpleCommandHandler());
            registry.Setup(x => x.GetPrioritisedCommandHandlers(It.IsAny<ICommand>())).Returns(
                new List<PrioritisedCommandHandler>
                {
                    new PrioritisedCommandHandler(0, typeof(SimpleCommandHandler))
                });            

            CommandExecuter executer = new CommandExecuter(registry.Object, handlerFactory.Object, scopeManager.Object, commandHandlerExecuter.Object, commandHandlerChainExecuter.Object);
            SimpleCommand simpleCommand = new SimpleCommand();

            // Act
            await executer.ExecuteAsync(simpleCommand);

            // Assert
            commandHandlerExecuter.Verify(x => x.ExecuteAsync(It.IsAny<ICommandHandler>(), simpleCommand, It.IsAny<SimpleResult>()));
        }

        [Fact]
        public async Task MissingCommandActorsThrowsException()
        {
            // Arrange
            Mock<ICommandHandlerFactory> handlerFactory = new Mock<ICommandHandlerFactory>();
            Mock<ICommandRegistry> registry = new Mock<ICommandRegistry>();
            Mock<ICommandHandlerExecuter> commandHandlerExecuter = new Mock<ICommandHandlerExecuter>();
            Mock<ICommandHandlerChainExecuter> commandHandlerChainExecuter = new Mock<ICommandHandlerChainExecuter>();
            Mock<ICommandScopeManager> scopeManager = new Mock<ICommandScopeManager>();
            handlerFactory.Setup(x => x.Create(typeof(SimpleCommandHandler))).Returns(new SimpleCommandHandler());
            registry.Setup(x => x.GetPrioritisedCommandHandlers(It.IsAny<ICommand>())).Returns<List<PrioritisedCommandHandler>>(null);

            CommandExecuter executer = new CommandExecuter(registry.Object, handlerFactory.Object, scopeManager.Object, commandHandlerExecuter.Object, commandHandlerChainExecuter.Object);

            // Act and assert
            MissingCommandHandlerRegistrationException ex = await Assert.ThrowsAsync<MissingCommandHandlerRegistrationException>(async () => await executer.ExecuteAsync(new SimpleCommand()));
            Assert.Equal(typeof(SimpleCommand), ex.CommandType);
        }

        [Fact]
        public async Task CommandChainHalts()
        {
            // Arrange
            Mock<ICommandHandlerFactory> handlerFactory = new Mock<ICommandHandlerFactory>();
            Mock<ICommandRegistry> registry = new Mock<ICommandRegistry>();
            Mock<ICommandHandlerExecuter> commandHandlerExecuter = new Mock<ICommandHandlerExecuter>();
            Mock<ICommandHandlerChainExecuter> commandHandlerChainExecuter = new Mock<ICommandHandlerChainExecuter>();
            Mock<ICommandScopeManager> scopeManager = new Mock<ICommandScopeManager>();
            handlerFactory.Setup(x => x.Create(typeof(SimpleCommandHandler))).Returns(new SimpleCommandHandler());
            handlerFactory.Setup(x => x.Create(typeof(SimpleCommandHandlerThatHalts))).Returns(new SimpleCommandHandlerThatHalts());
            handlerFactory.Setup(x => x.Create(typeof(SimpleCommandHandlerTwo))).Returns(new SimpleCommandHandlerTwo());
            registry.Setup(x => x.GetPrioritisedCommandHandlers(It.IsAny<ICommand>())).Returns(
                new List<PrioritisedCommandHandler>
                {
                    new PrioritisedCommandHandler(0, typeof(SimpleCommandHandler)),
                    new PrioritisedCommandHandler(1, typeof(SimpleCommandHandlerThatHalts)),
                    new PrioritisedCommandHandler(2, typeof(SimpleCommandHandlerTwo))
                });
            SimpleCommand simpleCommand = new SimpleCommand();
            commandHandlerChainExecuter
                .Setup(x => x.ExecuteAsync(It.IsAny<ICommandChainHandler>(), simpleCommand, It.IsAny<SimpleResult>()))
                .ReturnsAsync(new CommandChainHandlerResult<SimpleResult>(true, null));
            
            CommandExecuter executer = new CommandExecuter(registry.Object, handlerFactory.Object, scopeManager.Object, commandHandlerExecuter.Object, commandHandlerChainExecuter.Object);

            // Act
            SimpleResult result = await executer.ExecuteAsync(simpleCommand);

            // Assert
            // we should run the first SimpleCommandHandler as its not a chain command and won't be able to halt things and run the
            // SImpleCommandActorThatHalts once - it will halt and prevent the second non chained handler being called
            commandHandlerExecuter.Verify(x => x.ExecuteAsync(It.IsAny<ICommandHandler>(), simpleCommand, It.IsAny<SimpleResult>()), Times.Once);
            commandHandlerChainExecuter.Verify(x => x.ExecuteAsync(It.IsAny<ICommandChainHandler>(), simpleCommand, It.IsAny<SimpleResult>()), Times.Exactly(1));
        }

        [Fact]
        public async Task CommandChain()
        {
            // Arrange
            Mock<ICommandHandlerFactory> handlerFactory = new Mock<ICommandHandlerFactory>();
            Mock<ICommandRegistry> registry = new Mock<ICommandRegistry>();
            Mock<ICommandHandlerExecuter> commandHandlerExecuter = new Mock<ICommandHandlerExecuter>();
            Mock<ICommandHandlerChainExecuter> commandHandlerChainExecuter = new Mock<ICommandHandlerChainExecuter>();
            Mock<ICommandScopeManager> scopeManager = new Mock<ICommandScopeManager>();
            handlerFactory.Setup(x => x.Create(typeof(SimpleCommandHandler))).Returns(new SimpleCommandHandler());
            handlerFactory.Setup(x => x.Create(typeof(SimpleCommandHandlerTwo))).Returns(new SimpleCommandHandlerTwo());
            registry.Setup(x => x.GetPrioritisedCommandHandlers(It.IsAny<ICommand>())).Returns(
                new List<PrioritisedCommandHandler>
                {
                    new PrioritisedCommandHandler(0, typeof(SimpleCommandHandler)),
                    new PrioritisedCommandHandler(1, typeof(SimpleCommandHandlerTwo))
                });

            CommandExecuter executer = new CommandExecuter(registry.Object, handlerFactory.Object, scopeManager.Object, commandHandlerExecuter.Object, commandHandlerChainExecuter.Object);
            SimpleCommand simpleCommand = new SimpleCommand();

            // Act
            
            SimpleResult result = await executer.ExecuteAsync(simpleCommand);

            // Assert
            // if the third command had run their would be two items in the list and .single would throw an exception
            commandHandlerExecuter.Verify(x => x.ExecuteAsync(It.IsAny<ICommandHandler>(), simpleCommand, It.IsAny<SimpleResult>()), Times.Exactly(2));           
        }
    }
}
