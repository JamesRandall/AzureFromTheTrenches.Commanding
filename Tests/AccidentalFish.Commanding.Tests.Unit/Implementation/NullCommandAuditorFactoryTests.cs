using AccidentalFish.Commanding.Abstractions;
using AccidentalFish.Commanding.Implementation;
using AccidentalFish.Commanding.Tests.Unit.TestModel;
using Xunit;

namespace AccidentalFish.Commanding.Tests.Unit.Implementation
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
