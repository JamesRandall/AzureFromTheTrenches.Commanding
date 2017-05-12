using System;
using System.Linq;
using AccidentalFish.Commanding.Implementation;
using AccidentalFish.Commanding.Tests.Unit.TestModel;
using Moq;
using Xunit;

namespace AccidentalFish.Commanding.Tests.Unit.Implementation
{
    public class CommandRegistryTests
    {
        [Fact]
        public void SimpleRegistryIsRecorded()
        {
            // Arrange
            var registry = new CommandRegistry();

            // Act
            registry.Register<SimpleCommand, SimpleCommandActor>();

            // Assert
            var result = registry.GetPrioritisedCommandActors<SimpleCommand>();
            Assert.Equal(result.Single().CommandActorType, typeof(SimpleCommandActor));
        }

        [Fact]
        public void DispatcherIsRegisteredWithActor()
        {
            // Arrange
            var registry = new CommandRegistry();
            var dispatcher = new Mock<ICommandDispatcher>();

            // Act
            registry.Register<SimpleCommand, SimpleCommandActor>(dispatcher:dispatcher.Object);

            // Assert
            var result = registry.GetCommandDispatcher<SimpleCommand>();
            Assert.Equal(result, dispatcher.Object);
        }

        [Fact]
        public void DispatcherIsRegisteredWithoutActor()
        {
            // Arrange
            var registry = new CommandRegistry();
            var dispatcher = new Mock<ICommandDispatcher>();

            // Act
            registry.Register<SimpleCommand>(dispatcher.Object);

            // Assert
            var result = registry.GetCommandDispatcher<SimpleCommand>();
            Assert.Equal(result, dispatcher.Object);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void PriorityIsIndependentOfRegistryOrder(bool reverseRegistration)
        {
            // Arrange
            var registry = new CommandRegistry();

            // Act
            if (reverseRegistration)
            {
                registry.Register<SimpleCommand, SimpleCommandActorTwo>(order: 1500);
                registry.Register<SimpleCommand, SimpleCommandActor>(order: 1000);
            }
            else
            {
                registry.Register<SimpleCommand, SimpleCommandActor>(order: 1000);
                registry.Register<SimpleCommand, SimpleCommandActorTwo>(order: 1500);
            }
            

            // Assert
            var result = registry.GetPrioritisedCommandActors<SimpleCommand>();
            Assert.Collection(result, pca =>
            {
                if (pca.Priority != 1000) throw new Exception("Wrong order");
            }, pca =>
            {
                if (pca.Priority != 1500) throw new Exception("Wrong order");
            });
        }
    }
}
