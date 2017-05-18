using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            Mock<INoResultCommandActorBaseExecuter> noResultCommandActorBaseExecuter = new Mock<INoResultCommandActorBaseExecuter>();
            actorFactory.Setup(x => x.Create(typeof(SimpleCommandActor))).Returns(new SimpleCommandActor());
            registry.Setup(x => x.GetPrioritisedCommandActors<SimpleCommand>()).Returns(
                new List<PrioritisedCommandActor>
                {
                    new PrioritisedCommandActor(0, typeof(SimpleCommandActor))
                });

            CommandExecuter executer = new CommandExecuter(registry.Object, actorFactory.Object, noResultCommandActorBaseExecuter.Object);

            // Act
            SimpleResult result = await executer.ExecuteAsync<SimpleCommand, SimpleResult>(new SimpleCommand());

            // Assert
            Assert.Equal(typeof(SimpleCommandActor), result.Actors.Single());
        }

        [Fact]
        public async Task ExecutesWithTheNoResultType()
        {
            // Arrange
            Mock<ICommandActorFactory> actorFactory = new Mock<ICommandActorFactory>();
            Mock<ICommandRegistry> registry = new Mock<ICommandRegistry>();
            Mock<INoResultCommandActorBaseExecuter> noResultCommandActorBaseExecuter = new Mock<INoResultCommandActorBaseExecuter>();
            actorFactory.Setup(x => x.Create(typeof(SimpleCommandActor))).Returns(new SimpleCommandActor());
            registry.Setup(x => x.GetPrioritisedCommandActors<SimpleCommand>()).Returns(
                new List<PrioritisedCommandActor>
                {
                    new PrioritisedCommandActor(0, typeof(SimpleCommandActor))
                });

            CommandExecuter executer = new CommandExecuter(registry.Object, actorFactory.Object, noResultCommandActorBaseExecuter.Object);
            SimpleCommand command = new SimpleCommand();

            // Act
            await executer.ExecuteAsync<SimpleCommand, NoResult>(command);

            // Assert
            noResultCommandActorBaseExecuter.Verify(x => x.ExecuteAsync(It.IsAny<object>(), It.Is<object>(c => c == command)));
        }

        [Fact]
        public async Task UnexpectedResultTypeThrowsException()
        {
            // Arrange
            Mock<ICommandActorFactory> actorFactory = new Mock<ICommandActorFactory>();
            Mock<ICommandRegistry> registry = new Mock<ICommandRegistry>();
            Mock<INoResultCommandActorBaseExecuter> noResultCommandActorBaseExecuter = new Mock<INoResultCommandActorBaseExecuter>();
            actorFactory.Setup(x => x.Create(typeof(SimpleCommandActor))).Returns(new SimpleCommandActor());
            registry.Setup(x => x.GetPrioritisedCommandActors<SimpleCommand>()).Returns(
                new List<PrioritisedCommandActor>
                {
                    new PrioritisedCommandActor(0, typeof(SimpleCommandActor))
                });

            CommandExecuter executer = new CommandExecuter(registry.Object, actorFactory.Object, noResultCommandActorBaseExecuter.Object);

            // Act
            UnableToExecuteActorException ex = await Assert.ThrowsAsync<UnableToExecuteActorException>(async () => await executer.ExecuteAsync<SimpleCommand, SimpleCommand>(new SimpleCommand()));

            // Assert
            Assert.Equal("Unexpected result type", ex.Message);
        }

        [Fact]
        public async Task MissingCommandActorsThrowsException()
        {
            // Arrange
            Mock<ICommandActorFactory> actorFactory = new Mock<ICommandActorFactory>();
            Mock<ICommandRegistry> registry = new Mock<ICommandRegistry>();
            Mock<INoResultCommandActorBaseExecuter> noResultCommandActorBaseExecuter = new Mock<INoResultCommandActorBaseExecuter>();
            actorFactory.Setup(x => x.Create(typeof(SimpleCommandActor))).Returns(new SimpleCommandActor());
            registry.Setup(x => x.GetPrioritisedCommandActors<SimpleCommand>()).Returns<List<PrioritisedCommandActor>>(null);

            CommandExecuter executer = new CommandExecuter(registry.Object, actorFactory.Object, noResultCommandActorBaseExecuter.Object);

            // Act and assert
            MissingCommandActorRegistrationException ex = await Assert.ThrowsAsync<MissingCommandActorRegistrationException>(async () => await executer.ExecuteAsync<SimpleCommand, SimpleResult>(new SimpleCommand()));
            Assert.Equal(typeof(SimpleCommand), ex.CommandType);
        }

        [Fact]
        public async Task CommandChainHalts()
        {
            // Arrange
            Mock<ICommandActorFactory> actorFactory = new Mock<ICommandActorFactory>();
            Mock<ICommandRegistry> registry = new Mock<ICommandRegistry>();
            Mock<INoResultCommandActorBaseExecuter> noResultCommandActorBaseExecuter = new Mock<INoResultCommandActorBaseExecuter>();
            actorFactory.Setup(x => x.Create(typeof(SimpleCommandActor))).Returns(new SimpleCommandActor());
            actorFactory.Setup(x => x.Create(typeof(SimpleCommandActorThatHalts))).Returns(new SimpleCommandActorThatHalts());
            actorFactory.Setup(x => x.Create(typeof(SimpleCommandActorTwo))).Returns(new SimpleCommandActorTwo());
            registry.Setup(x => x.GetPrioritisedCommandActors<SimpleCommand>()).Returns(
                new List<PrioritisedCommandActor>
                {
                    new PrioritisedCommandActor(0, typeof(SimpleCommandActor)),
                    new PrioritisedCommandActor(1, typeof(SimpleCommandActorThatHalts)),
                    new PrioritisedCommandActor(2, typeof(SimpleCommandActorTwo))
                });

            CommandExecuter executer = new CommandExecuter(registry.Object, actorFactory.Object, noResultCommandActorBaseExecuter.Object);

            // Act
            SimpleResult result = await executer.ExecuteAsync<SimpleCommand, SimpleResult>(new SimpleCommand());

            // Assert
            // if the third command had run their would be two items in the list and .single would throw an exception
            Assert.Equal(typeof(SimpleCommandActor), result.Actors.Single()); 
        }

        [Fact]
        public async Task CommandChain()
        {
            // Arrange
            Mock<ICommandActorFactory> actorFactory = new Mock<ICommandActorFactory>();
            Mock<ICommandRegistry> registry = new Mock<ICommandRegistry>();
            Mock<INoResultCommandActorBaseExecuter> noResultCommandActorBaseExecuter = new Mock<INoResultCommandActorBaseExecuter>();
            actorFactory.Setup(x => x.Create(typeof(SimpleCommandActor))).Returns(new SimpleCommandActor());
            actorFactory.Setup(x => x.Create(typeof(SimpleCommandActorTwo))).Returns(new SimpleCommandActorTwo());
            registry.Setup(x => x.GetPrioritisedCommandActors<SimpleCommand>()).Returns(
                new List<PrioritisedCommandActor>
                {
                    new PrioritisedCommandActor(0, typeof(SimpleCommandActor)),
                    new PrioritisedCommandActor(1, typeof(SimpleCommandActorTwo))
                });

            CommandExecuter executer = new CommandExecuter(registry.Object, actorFactory.Object, noResultCommandActorBaseExecuter.Object);

            // Act
            SimpleResult result = await executer.ExecuteAsync<SimpleCommand, SimpleResult>(new SimpleCommand());

            // Assert
            // if the third command had run their would be two items in the list and .single would throw an exception
            Assert.Equal(typeof(SimpleCommandActor), result.Actors.First());
            Assert.Equal(typeof(SimpleCommandActorTwo), result.Actors.Last());
            Assert.Equal(2, result.Actors.Count);
        }
    }
}
