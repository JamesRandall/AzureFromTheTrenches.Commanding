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
            Mock<ICommandHandlerFactory> actorFactory = new Mock<ICommandHandlerFactory>();
            Mock<ICommandRegistry> registry = new Mock<ICommandRegistry>();
            Mock<ICommandActorExecuter> commandActorExecuter = new Mock<ICommandActorExecuter>();
            Mock<ICommandActorChainExecuter> commandActorChainExecuter = new Mock<ICommandActorChainExecuter>();
            Mock<ICommandScopeManager> scopeManager = new Mock<ICommandScopeManager>();
            actorFactory.Setup(x => x.Create(typeof(SimpleCommandHandler))).Returns(new SimpleCommandHandler());
            registry.Setup(x => x.GetPrioritisedCommandHandlers(It.IsAny<ICommand>())).Returns(
                new List<PrioritisedCommandHandler>
                {
                    new PrioritisedCommandHandler(0, typeof(SimpleCommandHandler))
                });            

            CommandExecuter executer = new CommandExecuter(registry.Object, actorFactory.Object, scopeManager.Object, commandActorExecuter.Object, commandActorChainExecuter.Object);
            SimpleCommand simpleCommand = new SimpleCommand();

            // Act
            await executer.ExecuteAsync(simpleCommand);

            // Assert
            commandActorExecuter.Verify(x => x.ExecuteAsync(It.IsAny<ICommandHandler>(), simpleCommand, It.IsAny<SimpleResult>()));
        }

        [Fact]
        public async Task MissingCommandActorsThrowsException()
        {
            // Arrange
            Mock<ICommandHandlerFactory> actorFactory = new Mock<ICommandHandlerFactory>();
            Mock<ICommandRegistry> registry = new Mock<ICommandRegistry>();
            Mock<ICommandActorExecuter> commandActorExecuter = new Mock<ICommandActorExecuter>();
            Mock<ICommandActorChainExecuter> commandActorChainExecuter = new Mock<ICommandActorChainExecuter>();
            Mock<ICommandScopeManager> scopeManager = new Mock<ICommandScopeManager>();
            actorFactory.Setup(x => x.Create(typeof(SimpleCommandHandler))).Returns(new SimpleCommandHandler());
            registry.Setup(x => x.GetPrioritisedCommandHandlers(It.IsAny<ICommand>())).Returns<List<PrioritisedCommandHandler>>(null);

            CommandExecuter executer = new CommandExecuter(registry.Object, actorFactory.Object, scopeManager.Object, commandActorExecuter.Object, commandActorChainExecuter.Object);

            // Act and assert
            MissingCommandActorRegistrationException ex = await Assert.ThrowsAsync<MissingCommandActorRegistrationException>(async () => await executer.ExecuteAsync(new SimpleCommand()));
            Assert.Equal(typeof(SimpleCommand), ex.CommandType);
        }

        [Fact]
        public async Task CommandChainHalts()
        {
            // Arrange
            Mock<ICommandHandlerFactory> actorFactory = new Mock<ICommandHandlerFactory>();
            Mock<ICommandRegistry> registry = new Mock<ICommandRegistry>();
            Mock<ICommandActorExecuter> commandActorExecuter = new Mock<ICommandActorExecuter>();
            Mock<ICommandActorChainExecuter> commandActorChainExecuter = new Mock<ICommandActorChainExecuter>();
            Mock<ICommandScopeManager> scopeManager = new Mock<ICommandScopeManager>();
            actorFactory.Setup(x => x.Create(typeof(SimpleCommandHandler))).Returns(new SimpleCommandHandler());
            actorFactory.Setup(x => x.Create(typeof(SimpleCommandHandlerThatHalts))).Returns(new SimpleCommandHandlerThatHalts());
            actorFactory.Setup(x => x.Create(typeof(SimpleCommandHandlerTwo))).Returns(new SimpleCommandHandlerTwo());
            registry.Setup(x => x.GetPrioritisedCommandHandlers(It.IsAny<ICommand>())).Returns(
                new List<PrioritisedCommandHandler>
                {
                    new PrioritisedCommandHandler(0, typeof(SimpleCommandHandler)),
                    new PrioritisedCommandHandler(1, typeof(SimpleCommandHandlerThatHalts)),
                    new PrioritisedCommandHandler(2, typeof(SimpleCommandHandlerTwo))
                });
            SimpleCommand simpleCommand = new SimpleCommand();
            commandActorChainExecuter
                .Setup(x => x.ExecuteAsync(It.IsAny<ICommandChainHandler>(), simpleCommand, It.IsAny<SimpleResult>()))
                .ReturnsAsync(new CommandChainHandlerResult<SimpleResult>(true, null));
            
            CommandExecuter executer = new CommandExecuter(registry.Object, actorFactory.Object, scopeManager.Object, commandActorExecuter.Object, commandActorChainExecuter.Object);

            // Act
            SimpleResult result = await executer.ExecuteAsync(simpleCommand);

            // Assert
            // we should run the first SimpleCommandHandler as its not a chain command and won't be able to halt things and run the
            // SImpleCommandActorThatHalts once - it will halt and prevent the second non chained handler being called
            commandActorExecuter.Verify(x => x.ExecuteAsync(It.IsAny<ICommandHandler>(), simpleCommand, It.IsAny<SimpleResult>()), Times.Once);
            commandActorChainExecuter.Verify(x => x.ExecuteAsync(It.IsAny<ICommandChainHandler>(), simpleCommand, It.IsAny<SimpleResult>()), Times.Exactly(1));
        }

        [Fact]
        public async Task CommandChain()
        {
            // Arrange
            Mock<ICommandHandlerFactory> actorFactory = new Mock<ICommandHandlerFactory>();
            Mock<ICommandRegistry> registry = new Mock<ICommandRegistry>();
            Mock<ICommandActorExecuter> commandActorExecuter = new Mock<ICommandActorExecuter>();
            Mock<ICommandActorChainExecuter> commandActorChainExecuter = new Mock<ICommandActorChainExecuter>();
            Mock<ICommandScopeManager> scopeManager = new Mock<ICommandScopeManager>();
            actorFactory.Setup(x => x.Create(typeof(SimpleCommandHandler))).Returns(new SimpleCommandHandler());
            actorFactory.Setup(x => x.Create(typeof(SimpleCommandHandlerTwo))).Returns(new SimpleCommandHandlerTwo());
            registry.Setup(x => x.GetPrioritisedCommandHandlers(It.IsAny<ICommand>())).Returns(
                new List<PrioritisedCommandHandler>
                {
                    new PrioritisedCommandHandler(0, typeof(SimpleCommandHandler)),
                    new PrioritisedCommandHandler(1, typeof(SimpleCommandHandlerTwo))
                });

            CommandExecuter executer = new CommandExecuter(registry.Object, actorFactory.Object, scopeManager.Object, commandActorExecuter.Object, commandActorChainExecuter.Object);
            SimpleCommand simpleCommand = new SimpleCommand();

            // Act
            
            SimpleResult result = await executer.ExecuteAsync(simpleCommand);

            // Assert
            // if the third command had run their would be two items in the list and .single would throw an exception
            commandActorExecuter.Verify(x => x.ExecuteAsync(It.IsAny<ICommandHandler>(), simpleCommand, It.IsAny<SimpleResult>()), Times.Exactly(2));           
        }
    }
}
