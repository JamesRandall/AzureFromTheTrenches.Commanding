using System.Threading.Tasks;
using Moq;
using Xunit;
using AccidentalFish.Commanding.Cache.Implementation;
using AccidentalFish.Commanding.Cache.Tests.Unit.TestModel;

namespace AccidentalFish.Commanding.Cache.Tests.Unit.Implementation
{
    public class CachedCommandDispatcherTests
    {
        [Fact]
        public async Task DispatchesThroughPipelineWhenNoConfiguredOptionsForCommandType()
        {
            // Arrange
            Mock<ICacheKeyProvider> cacheKeyProvider = new Mock<ICacheKeyProvider>();
            Mock<ICommandDispatcher> underlyingCommandDispatcher = new Mock<ICommandDispatcher>();
            Mock< ICacheOptionsProvider> cacheOptionsProvider = new Mock<ICacheOptionsProvider>();
            Mock<ICacheWrapper> cacheWrapper = new Mock<ICacheWrapper>();
            SimpleCommand command = new SimpleCommand();

            CachedCommandDispatcher dispatcher = new CachedCommandDispatcher(cacheKeyProvider.Object, underlyingCommandDispatcher.Object, cacheOptionsProvider.Object, cacheWrapper.Object);

            // Act
            await dispatcher.DispatchAsync<SimpleCommand, SimpleResult>(command);

            // Assert
            underlyingCommandDispatcher.Verify(x => x.DispatchAsync<SimpleCommand, SimpleResult>(command));
            cacheWrapper.Verify(x => x.Get<SimpleResult>(It.IsAny<string>()), Times.Never);
        }
    }
}
