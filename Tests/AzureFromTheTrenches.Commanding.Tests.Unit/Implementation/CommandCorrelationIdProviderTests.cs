using System;
using AzureFromTheTrenches.Commanding.Implementation;
using Xunit;

namespace AzureFromTheTrenches.Commanding.Tests.Unit.Implementation
{
    public class CommandCorrelationIdProviderTests
    {
        [Fact]
        public void EnsureCorrelationIdIsBasedOnGuidAndNotEmpty()
        {
            // Arrange
            CommandCorrelationIdProvider provider = new CommandCorrelationIdProvider();

            // Act
            string id = provider.Create();

            // Assert
            Assert.NotEqual(Guid.Empty, Guid.Parse(id));
        }

        [Fact]
        public void RepeatCallsCreateDifferentIds()
        {
            // Arrange
            CommandCorrelationIdProvider provider = new CommandCorrelationIdProvider();

            // Act
            string firstId = provider.Create();
            string secondId = provider.Create();

            // Assert
            Assert.NotEqual(firstId, secondId);
        }
    }
}
