using System.Threading;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;
using AzureFromTheTrenches.Commanding.Http.Implementation;
using AzureFromTheTrenches.Commanding.Http.Tests.Unit.TestInfrastructure;
using Moq;
using Xunit;

namespace AzureFromTheTrenches.Commanding.Http.Tests.Unit.Implementation
{
    public class HttpCommandDispatcherShould
    {
        private readonly Mock<ICommandExecuter> _commandExecuter = new Mock<ICommandExecuter>();

        [Fact]
        public async Task ReturnNotDefferedCommandResultForCommandWithResult()
        {
            HttpCommandDispatcher testSubject = new HttpCommandDispatcher(_commandExecuter.Object);
            CommandResult<int> commandResult =
                await testSubject.DispatchAsync(new SimpleCommandWithIntegerResult(), default(CancellationToken));

            Assert.False(commandResult.DeferExecution);
            Assert.Equal(default(int), commandResult.Result);
        }

        [Fact]
        public async Task ReturnNotDeferredCommandResultForCommandWithNoResult()
        {
            HttpCommandDispatcher testSubject = new HttpCommandDispatcher(_commandExecuter.Object);
            CommandResult commandResult = await testSubject.DispatchAsync(new SimpleCommand(), default(CancellationToken));

            Assert.False(commandResult.DeferExecution);
        }

        [Fact]
        public void HaveAssociatedExecuter()
        {
            HttpCommandDispatcher testSubject = new HttpCommandDispatcher(_commandExecuter.Object);

            Assert.Equal(_commandExecuter.Object, testSubject.AssociatedExecuter);
        }
    }
}
