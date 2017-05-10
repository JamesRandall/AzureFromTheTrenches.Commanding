using System;
using System.Linq;
using AccidentalFish.Commanding.Implementation;
using AccidentalFish.Commanding.Tests.Unit.Model;
using Xunit;

namespace AccidentalFish.Commanding.Tests.Unit
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
