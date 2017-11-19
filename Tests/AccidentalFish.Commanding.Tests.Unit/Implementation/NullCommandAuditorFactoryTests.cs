using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Implementation;
using AzureFromTheTrenches.Commanding.Tests.Unit.TestModel;
using Xunit;

namespace AzureFromTheTrenches.Commanding.Tests.Unit.Implementation
{
    public class NullCommandAuditorFactoryTests
    {
        [Fact]
        public void ReturnsNullAuditor()
        {
            // Arrange
            NullCommandAuditorFactory factory = new NullCommandAuditorFactory();

            // Act
            ICommandAuditor auditor = factory.Create<SimpleCommand>();

            // Assert
            Assert.Null(auditor);
        }
    }
}
