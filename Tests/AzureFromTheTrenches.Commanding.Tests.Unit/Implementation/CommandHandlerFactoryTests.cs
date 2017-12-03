using AzureFromTheTrenches.Commanding.Implementation;
using Xunit;

namespace AzureFromTheTrenches.Commanding.Tests.Unit.Implementation
{
    public class CommandHandlerFactoryTests
    {
        [Fact]
        public void CreatesHandler()
        {
            object testResult = new object();
            CommandHandlerFactory testSubject = new CommandHandlerFactory(t => testResult);

            object result = testSubject.Create(typeof(string));

            Assert.Same(testResult, result);
        }
    }
}
