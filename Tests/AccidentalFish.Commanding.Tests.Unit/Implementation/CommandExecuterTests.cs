using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccidentalFish.Commanding.Implementation;
using AccidentalFish.Commanding.Model;
using AccidentalFish.Commanding.Tests.Unit.TestModel;
using AccidentalFish.DependencyResolver;
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
            Mock<IDependencyResolver> resolver = new Mock<IDependencyResolver>();
            Mock<ICommandRegistry> registry = new Mock<ICommandRegistry>();
            resolver.Setup(x => x.Resolve(typeof(SimpleCommandActor))).Returns(new SimpleCommandActor());
            registry.Setup(x => x.GetPrioritisedCommandActors<SimpleCommand>()).Returns(
                new List<PrioritisedCommandActor>
                {
                    new PrioritisedCommandActor(0, typeof(SimpleCommandActor))
                });

            CommandExecuter executer = new CommandExecuter(resolver.Object, registry.Object);

            // Act
            SimpleResult result = await executer.ExecuteAsync<SimpleCommand, SimpleResult>(new SimpleCommand());

            // Assert
            Assert.Equal(result.Actors.Single(), typeof(SimpleCommandActor));
        }

        [Fact]
        public async Task MissingCommandActorsThrowsException()
        {
            // Arrange
            Mock<IDependencyResolver> resolver = new Mock<IDependencyResolver>();
            Mock<ICommandRegistry> registry = new Mock<ICommandRegistry>();
            resolver.Setup(x => x.Resolve(typeof(SimpleCommandActor))).Returns(new SimpleCommandActor());
            registry.Setup(x => x.GetPrioritisedCommandActors<SimpleCommand>()).Returns<List<PrioritisedCommandActor>>(null);

            CommandExecuter executer = new CommandExecuter(resolver.Object, registry.Object);

            // Act and assert
            MissingCommandActorRegistrationException ex = await Assert.ThrowsAsync<MissingCommandActorRegistrationException>(async () => await executer.ExecuteAsync<SimpleCommand, SimpleResult>(new SimpleCommand()));
            Assert.Equal(ex.CommandType, typeof(SimpleCommand));
        }

        [Fact]
        public async Task CommandChainHalts()
        {
            // Arrange
            Mock<IDependencyResolver> resolver = new Mock<IDependencyResolver>();
            Mock<ICommandRegistry> registry = new Mock<ICommandRegistry>();
            resolver.Setup(x => x.Resolve(typeof(SimpleCommandActor))).Returns(new SimpleCommandActor());
            resolver.Setup(x => x.Resolve(typeof(SimpleCommandActorThatHalts))).Returns(new SimpleCommandActorThatHalts());
            resolver.Setup(x => x.Resolve(typeof(SimpleCommandActorTwo))).Returns(new SimpleCommandActorTwo());
            registry.Setup(x => x.GetPrioritisedCommandActors<SimpleCommand>()).Returns(
                new List<PrioritisedCommandActor>
                {
                    new PrioritisedCommandActor(0, typeof(SimpleCommandActor)),
                    new PrioritisedCommandActor(1, typeof(SimpleCommandActorThatHalts)),
                    new PrioritisedCommandActor(2, typeof(SimpleCommandActorTwo))
                });

            CommandExecuter executer = new CommandExecuter(resolver.Object, registry.Object);

            // Act
            SimpleResult result = await executer.ExecuteAsync<SimpleCommand, SimpleResult>(new SimpleCommand());

            // Assert
            // if the third command had run their would be two items in the list and .single would throw an exception
            Assert.Equal(result.Actors.Single(), typeof(SimpleCommandActor)); 
        }

        [Fact]
        public async Task CommandChain()
        {
            // Arrange
            Mock<IDependencyResolver> resolver = new Mock<IDependencyResolver>();
            Mock<ICommandRegistry> registry = new Mock<ICommandRegistry>();
            resolver.Setup(x => x.Resolve(typeof(SimpleCommandActor))).Returns(new SimpleCommandActor());
            resolver.Setup(x => x.Resolve(typeof(SimpleCommandActorTwo))).Returns(new SimpleCommandActorTwo());
            registry.Setup(x => x.GetPrioritisedCommandActors<SimpleCommand>()).Returns(
                new List<PrioritisedCommandActor>
                {
                    new PrioritisedCommandActor(0, typeof(SimpleCommandActor)),
                    new PrioritisedCommandActor(1, typeof(SimpleCommandActorTwo))
                });

            CommandExecuter executer = new CommandExecuter(resolver.Object, registry.Object);

            // Act
            SimpleResult result = await executer.ExecuteAsync<SimpleCommand, SimpleResult>(new SimpleCommand());

            // Assert
            // if the third command had run their would be two items in the list and .single would throw an exception
            Assert.Equal(result.Actors.First(), typeof(SimpleCommandActor));
            Assert.Equal(result.Actors.Last(), typeof(SimpleCommandActorTwo));
            Assert.Equal(result.Actors.Count, 2);
        }
    }
}
