using System;
using System.Linq;
using AccidentalFish.Commanding.Abstractions;
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
            registry.Register<SimpleCommandActor>();

            // Assert
            var result = registry.GetPrioritisedCommandActors(new SimpleCommand());
            Assert.Equal(typeof(SimpleCommandActor), result.Single().CommandActorType);
        }

        [Fact]
        public void DispatcherIsRegisteredWithActor()
        {
            // Arrange
            var registry = new CommandRegistry();
            ICommandDispatcher DispatcherFunc() => new Mock<ICommandDispatcher>().Object;

            // Act
            registry.Register<SimpleCommandActor>(dispatcherFactoryFunc:DispatcherFunc);

            // Assert
            var result = registry.GetCommandDispatcherFactory(new SimpleCommand());
            Assert.Equal(result, DispatcherFunc);
        }

        [Fact]
        public void DispatcherIsRegisteredWithoutActor()
        {
            // Arrange
            var registry = new CommandRegistry();
            ICommandDispatcher DispatcherFunc() => new Mock<ICommandDispatcher>().Object;

            // Act
            registry.Register<SimpleCommand, SimpleResult>(DispatcherFunc);

            // Assert
            var result = registry.GetCommandDispatcherFactory(new SimpleCommand());
            Assert.Equal(result, DispatcherFunc);
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
                registry.Register<SimpleCommandActorTwo>(order: 1500);
                registry.Register<SimpleCommandActor>(order: 1000);
            }
            else
            {
                registry.Register<SimpleCommandActor>(order: 1000);
                registry.Register<SimpleCommandActorTwo>(order: 1500);
            }
            

            // Assert
            var result = registry.GetPrioritisedCommandActors(new SimpleCommand());
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
