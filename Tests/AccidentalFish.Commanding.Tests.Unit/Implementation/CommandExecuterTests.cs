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
    public class CommandExecuterTests
    {
        [Fact]
        public async Task SimpleCommandExecutes()
        {
            // Arrange
            Mock<ICommandActorFactory> actorFactory = new Mock<ICommandActorFactory>();
            Mock<ICommandRegistry> registry = new Mock<ICommandRegistry>();
            Mock<ICommandActorExecuter> commandActorExecuter = new Mock<ICommandActorExecuter>();
            Mock<ICommandActorChainExecuter> commandActorChainExecuter = new Mock<ICommandActorChainExecuter>();
            Mock<ICommandScopeManager> scopeManager = new Mock<ICommandScopeManager>();
            actorFactory.Setup(x => x.Create(typeof(SimpleCommandActor))).Returns(new SimpleCommandActor());
            registry.Setup(x => x.GetPrioritisedCommandActors(It.IsAny<ICommand>())).Returns(
                new List<PrioritisedCommandActor>
                {
                    new PrioritisedCommandActor(0, typeof(SimpleCommandActor))
                });            

            CommandExecuter executer = new CommandExecuter(registry.Object, actorFactory.Object, scopeManager.Object, commandActorExecuter.Object, commandActorChainExecuter.Object);
            SimpleCommand simpleCommand = new SimpleCommand();

            // Act
            await executer.ExecuteAsync(simpleCommand);

            // Assert
            commandActorExecuter.Verify(x => x.ExecuteAsync(It.IsAny<ICommandActor>(), simpleCommand, It.IsAny<SimpleResult>()));
        }

        [Fact]
        public async Task MissingCommandActorsThrowsException()
        {
            // Arrange
            Mock<ICommandActorFactory> actorFactory = new Mock<ICommandActorFactory>();
            Mock<ICommandRegistry> registry = new Mock<ICommandRegistry>();
            Mock<ICommandActorExecuter> commandActorExecuter = new Mock<ICommandActorExecuter>();
            Mock<ICommandActorChainExecuter> commandActorChainExecuter = new Mock<ICommandActorChainExecuter>();
            Mock<ICommandScopeManager> scopeManager = new Mock<ICommandScopeManager>();
            actorFactory.Setup(x => x.Create(typeof(SimpleCommandActor))).Returns(new SimpleCommandActor());
            registry.Setup(x => x.GetPrioritisedCommandActors(It.IsAny<ICommand>())).Returns<List<PrioritisedCommandActor>>(null);

            CommandExecuter executer = new CommandExecuter(registry.Object, actorFactory.Object, scopeManager.Object, commandActorExecuter.Object, commandActorChainExecuter.Object);

            // Act and assert
            MissingCommandActorRegistrationException ex = await Assert.ThrowsAsync<MissingCommandActorRegistrationException>(async () => await executer.ExecuteAsync(new SimpleCommand()));
            Assert.Equal(typeof(SimpleCommand), ex.CommandType);
        }

        [Fact]
        public async Task CommandChainHalts()
        {
            // Arrange
            Mock<ICommandActorFactory> actorFactory = new Mock<ICommandActorFactory>();
            Mock<ICommandRegistry> registry = new Mock<ICommandRegistry>();
            Mock<ICommandActorExecuter> commandActorExecuter = new Mock<ICommandActorExecuter>();
            Mock<ICommandActorChainExecuter> commandActorChainExecuter = new Mock<ICommandActorChainExecuter>();
            Mock<ICommandScopeManager> scopeManager = new Mock<ICommandScopeManager>();
            actorFactory.Setup(x => x.Create(typeof(SimpleCommandActor))).Returns(new SimpleCommandActor());
            actorFactory.Setup(x => x.Create(typeof(SimpleCommandActorThatHalts))).Returns(new SimpleCommandActorThatHalts());
            actorFactory.Setup(x => x.Create(typeof(SimpleCommandActorTwo))).Returns(new SimpleCommandActorTwo());
            registry.Setup(x => x.GetPrioritisedCommandActors(It.IsAny<ICommand>())).Returns(
                new List<PrioritisedCommandActor>
                {
                    new PrioritisedCommandActor(0, typeof(SimpleCommandActor)),
                    new PrioritisedCommandActor(1, typeof(SimpleCommandActorThatHalts)),
                    new PrioritisedCommandActor(2, typeof(SimpleCommandActorTwo))
                });
            SimpleCommand simpleCommand = new SimpleCommand();
            commandActorChainExecuter
                .Setup(x => x.ExecuteAsync(It.IsAny<ICommandChainActor>(), simpleCommand, It.IsAny<SimpleResult>()))
                .ReturnsAsync(new CommandChainActorResult<SimpleResult>(true, null));
            
            CommandExecuter executer = new CommandExecuter(registry.Object, actorFactory.Object, scopeManager.Object, commandActorExecuter.Object, commandActorChainExecuter.Object);

            // Act
            SimpleResult result = await executer.ExecuteAsync(simpleCommand);

            // Assert
            // we should run the first SimpleCommandActor as its not a chain command and won't be able to halt things and run the
            // SImpleCommandActorThatHalts once - it will halt and prevent the second non chained actor being called
            commandActorExecuter.Verify(x => x.ExecuteAsync(It.IsAny<ICommandActor>(), simpleCommand, It.IsAny<SimpleResult>()), Times.Once);
            commandActorChainExecuter.Verify(x => x.ExecuteAsync(It.IsAny<ICommandChainActor>(), simpleCommand, It.IsAny<SimpleResult>()), Times.Exactly(1));
        }

        [Fact]
        public async Task CommandChain()
        {
            // Arrange
            Mock<ICommandActorFactory> actorFactory = new Mock<ICommandActorFactory>();
            Mock<ICommandRegistry> registry = new Mock<ICommandRegistry>();
            Mock<ICommandActorExecuter> commandActorExecuter = new Mock<ICommandActorExecuter>();
            Mock<ICommandActorChainExecuter> commandActorChainExecuter = new Mock<ICommandActorChainExecuter>();
            Mock<ICommandScopeManager> scopeManager = new Mock<ICommandScopeManager>();
            actorFactory.Setup(x => x.Create(typeof(SimpleCommandActor))).Returns(new SimpleCommandActor());
            actorFactory.Setup(x => x.Create(typeof(SimpleCommandActorTwo))).Returns(new SimpleCommandActorTwo());
            registry.Setup(x => x.GetPrioritisedCommandActors(It.IsAny<ICommand>())).Returns(
                new List<PrioritisedCommandActor>
                {
                    new PrioritisedCommandActor(0, typeof(SimpleCommandActor)),
                    new PrioritisedCommandActor(1, typeof(SimpleCommandActorTwo))
                });

            CommandExecuter executer = new CommandExecuter(registry.Object, actorFactory.Object, scopeManager.Object, commandActorExecuter.Object, commandActorChainExecuter.Object);
            SimpleCommand simpleCommand = new SimpleCommand();

            // Act
            
            SimpleResult result = await executer.ExecuteAsync(simpleCommand);

            // Assert
            // if the third command had run their would be two items in the list and .single would throw an exception
            commandActorExecuter.Verify(x => x.ExecuteAsync(It.IsAny<ICommandActor>(), simpleCommand, It.IsAny<SimpleResult>()), Times.Exactly(2));           
        }
    }
}
