using System;
using System.Linq;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Implementation;
using AzureFromTheTrenches.Commanding.Tests.Unit.TestModel;
using Moq;
using Xunit;

namespace AzureFromTheTrenches.Commanding.Tests.Unit.Implementation
{
    public class CommandRegistryTests
    {
        [Fact]
        public void SimpleCommandHandlerIsRegisteredByGenericType()
        {
            // Arrange
            var registry = new CommandRegistry(new Mock<ICommandHandlerExecuter>().Object);

            // Act
            registry.Register<SimpleCommandHandler>();

            // Assert
            var result = registry.GetPrioritisedCommandHandlers(new SimpleCommand());
            Assert.Equal(typeof(SimpleCommandHandler), result.Single().CommandHandlerType);
        }
        
        [Fact]
        public void SimpleCommandHandlerIsRegisteredByObjectType()
        {
            // Arrange
            var registry = new CommandRegistry(new Mock<ICommandHandlerExecuter>().Object);

            // Act
            registry.Register(typeof(SimpleCommandHandler));

            // Assert
            var result = registry.GetPrioritisedCommandHandlers(new SimpleCommand());
            Assert.Equal(typeof(SimpleCommandHandler), result.Single().CommandHandlerType);
        }
        
        [Fact]
        public void SimpleCommandHandlerThatDoesntImplementBaseClassThrowsCommandRegistrationException()
        {
            // Arrange
            var registry = new CommandRegistry(new Mock<ICommandHandlerExecuter>().Object);

            // Act / Assert
            Assert.Throws<CommandRegistrationException>(() =>
            {
                registry.Register(typeof(SimpleCommandNoImplementaion));
            });
        }

        [Fact]
        public void DispatcherIsRegisteredWithHandler()
        {
            // Arrange
            var registry = new CommandRegistry(new Mock<ICommandHandlerExecuter>().Object);
            ICommandDispatcher DispatcherFunc() => new Mock<ICommandDispatcher>().Object;

            // Act
            registry.Register<SimpleCommandHandler>(1000, dispatcherFactoryFunc:DispatcherFunc);

            // Assert
            var result = registry.GetCommandDispatcherFactory(new SimpleCommand());
            Assert.Equal(result, DispatcherFunc);
        }

        [Fact]
        public void DispatcherIsRegisteredWithoutHandler()
        {
            // Arrange
            var registry = new CommandRegistry(new Mock<ICommandHandlerExecuter>().Object);
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
            var registry = new CommandRegistry(new Mock<ICommandHandlerExecuter>().Object);

            // Act
            if (reverseRegistration)
            {
                registry.Register<SimpleCommandHandlerTwo>(order: 1500);
                registry.Register<SimpleCommandHandler>(order: 1000);
            }
            else
            {
                registry.Register<SimpleCommandHandler>(order: 1000);
                registry.Register<SimpleCommandHandlerTwo>(order: 1500);
            }
            

            // Assert
            var result = registry.GetPrioritisedCommandHandlers(new SimpleCommand());
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
