using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Implementation;
using AzureFromTheTrenches.Commanding.Tests.Unit.TestModel;
using Xunit;

namespace AzureFromTheTrenches.Commanding.Tests.Unit.Implementation
{
    public class NoResultCommandActorBaseExecuterTests
    {
        [Fact]
        public async Task CompilesExpressionAndExecutesActor()
        {
            // Arrange
            INoResultCommandActorBaseExecuter executer = new NoResultCommandActorBaseExecuter();

            // Act
            SimpleCommand command = new SimpleCommand();
            await executer.ExecuteAsync(new MutateSimpleCommandActor(), command);

            // Assert
            Assert.Equal("i did mutate it", command.Message);
        }
    }
}
