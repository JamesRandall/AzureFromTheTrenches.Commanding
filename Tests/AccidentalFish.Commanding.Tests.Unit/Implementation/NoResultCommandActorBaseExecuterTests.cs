using System.Threading.Tasks;
using AccidentalFish.Commanding.Implementation;
using AccidentalFish.Commanding.Tests.Unit.TestModel;
using Xunit;

namespace AccidentalFish.Commanding.Tests.Unit.Implementation
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
