using System;
using System.Threading.Tasks;
using Moq;
using Xunit;
using AccidentalFish.Commanding.Cache.Implementation;
using AccidentalFish.Commanding.Cache.Tests.Unit.TestModel;
using AccidentalFish.Commanding.Model;

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
            Mock<ICacheAdapter> cacheWrapper = new Mock<ICacheAdapter>();
            SimpleCommand command = new SimpleCommand();

            CachedCommandDispatcher dispatcher = new CachedCommandDispatcher(cacheKeyProvider.Object, underlyingCommandDispatcher.Object, cacheOptionsProvider.Object, cacheWrapper.Object);

            // Act
            await dispatcher.DispatchAsync<SimpleCommand, SimpleResult>(command);

            // Assert
            underlyingCommandDispatcher.Verify(x => x.DispatchAsync<SimpleCommand, SimpleResult>(command));
            cacheWrapper.Verify(x => x.Get<SimpleResult>(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task DoesntDispatchThroughPipelineOnCacheHit()
        {
            // Arrange
            Mock<ICacheKeyProvider> cacheKeyProvider = new Mock<ICacheKeyProvider>();
            Mock<ICommandDispatcher> underlyingCommandDispatcher = new Mock<ICommandDispatcher>();
            Mock<ICacheOptionsProvider> cacheOptionsProvider = new Mock<ICacheOptionsProvider>();
            Mock<ICacheAdapter> cacheWrapper = new Mock<ICacheAdapter>();
            CacheOptions options = new CacheOptions(typeof(SimpleCommand), TimeSpan.FromMinutes(5));
            SimpleCommand command = new SimpleCommand();
            SimpleResult cachedResult = new SimpleResult
            {
                ANumber = 2
            };
            cacheWrapper.Setup(x => x.Get<SimpleResult>(It.IsAny<string>())).ReturnsAsync(cachedResult);
            cacheOptionsProvider.Setup(x => x.Get(command)).Returns(options);
            cacheKeyProvider.Setup(x => x.CacheKey(command)).Returns("akey");

            CachedCommandDispatcher dispatcher = new CachedCommandDispatcher(cacheKeyProvider.Object, underlyingCommandDispatcher.Object, cacheOptionsProvider.Object, cacheWrapper.Object);

            // Act
            var result = await dispatcher.DispatchAsync<SimpleCommand, SimpleResult>(command);

            // Assert
            underlyingCommandDispatcher.Verify(x => x.DispatchAsync<SimpleCommand, SimpleResult>(command), Times.Never);
            Assert.Equal(2, result.Result.ANumber);
        }

        [Fact]
        public async Task DispatchesThroughPipelineOnCacheHitWithNoThrottling()
        {
            // Arrange
            Mock<ICacheKeyProvider> cacheKeyProvider = new Mock<ICacheKeyProvider>();
            Mock<ICommandDispatcher> underlyingCommandDispatcher = new Mock<ICommandDispatcher>();
            Mock<ICacheOptionsProvider> cacheOptionsProvider = new Mock<ICacheOptionsProvider>();
            Mock<ICacheAdapter> cacheWrapper = new Mock<ICacheAdapter>();
            CacheOptions options = new CacheOptions(typeof(SimpleCommand), TimeSpan.FromMinutes(5));
            SimpleCommand command = new SimpleCommand();
            SimpleResult cachedResult = new SimpleResult
            {
                ANumber = 2
            };
            cacheOptionsProvider.Setup(x => x.Get(command)).Returns(options);
            cacheKeyProvider.Setup(x => x.CacheKey(command)).Returns("akey");
            cacheWrapper.Setup(x => x.Set("akey", It.IsAny<object>(), TimeSpan.FromMinutes(5))).Returns(Task.FromResult(0));
            underlyingCommandDispatcher.Setup(x => x.DispatchAsync<SimpleCommand, SimpleResult>(command)).ReturnsAsync(new CommandResult<SimpleResult>(cachedResult, false));

            CachedCommandDispatcher dispatcher = new CachedCommandDispatcher(cacheKeyProvider.Object, underlyingCommandDispatcher.Object, cacheOptionsProvider.Object, cacheWrapper.Object);

            // Act
            var result = await dispatcher.DispatchAsync<SimpleCommand, SimpleResult>(command);

            // Assert
            underlyingCommandDispatcher.Verify(x => x.DispatchAsync<SimpleCommand, SimpleResult>(command), Times.Once, "Underlying dispatched was not called");
            cacheWrapper.Verify(x => x.Set("akey", cachedResult, TimeSpan.FromMinutes(5)), Times.Once, "Result was not set in cache");
            Assert.Equal(2, result.Result.ANumber);
        }

        [Fact]
        public async Task DispatchesThroughPipelineOnCacheHitWithThrottling()
        {
            // Arrange
            Mock<ICacheKeyProvider> cacheKeyProvider = new Mock<ICacheKeyProvider>();
            Mock<ICommandDispatcher> underlyingCommandDispatcher = new Mock<ICommandDispatcher>();
            Mock<ICacheOptionsProvider> cacheOptionsProvider = new Mock<ICacheOptionsProvider>();
            Mock<ICacheAdapter> cacheWrapper = new Mock<ICacheAdapter>();
            CacheOptions options = new CacheOptions(typeof(SimpleCommand), TimeSpan.FromMinutes(5), 1);
            SimpleCommand command = new SimpleCommand();
            SimpleResult cachedResult = new SimpleResult
            {
                ANumber = 2
            };
            cacheOptionsProvider.Setup(x => x.Get(command)).Returns(options);
            cacheKeyProvider.Setup(x => x.CacheKey(command)).Returns("akey");
            cacheWrapper.Setup(x => x.Set("akey", It.IsAny<object>(), TimeSpan.FromMinutes(5))).Returns(Task.FromResult(0));
            underlyingCommandDispatcher.Setup(x => x.DispatchAsync<SimpleCommand, SimpleResult>(command)).ReturnsAsync(new CommandResult<SimpleResult>(cachedResult, false));

            CachedCommandDispatcher dispatcher = new CachedCommandDispatcher(cacheKeyProvider.Object, underlyingCommandDispatcher.Object, cacheOptionsProvider.Object, cacheWrapper.Object);

            // Act
            var result = await dispatcher.DispatchAsync<SimpleCommand, SimpleResult>(command);

            // Assert
            underlyingCommandDispatcher.Verify(x => x.DispatchAsync<SimpleCommand, SimpleResult>(command), Times.Once, "Underlying dispatched was not called");
            cacheWrapper.Verify(x => x.Set("akey", cachedResult, TimeSpan.FromMinutes(5)), Times.Once, "Result was not set in cache");
            Assert.Equal(2, result.Result.ANumber);
        }

        [Fact]
        public async Task DispatchesThroughPipelineWhenNoResultExpected()
        {
            // Arrange
            Mock<ICacheKeyProvider> cacheKeyProvider = new Mock<ICacheKeyProvider>();
            Mock<ICommandDispatcher> underlyingCommandDispatcher = new Mock<ICommandDispatcher>();
            Mock<ICacheOptionsProvider> cacheOptionsProvider = new Mock<ICacheOptionsProvider>();
            Mock<ICacheAdapter> cacheWrapper = new Mock<ICacheAdapter>();
            SimpleCommand command = new SimpleCommand();

            CachedCommandDispatcher dispatcher = new CachedCommandDispatcher(cacheKeyProvider.Object, underlyingCommandDispatcher.Object, cacheOptionsProvider.Object, cacheWrapper.Object);

            // Act
            await dispatcher.DispatchAsync(command);

            // Assert
            underlyingCommandDispatcher.Verify(x => x.DispatchAsync(command));
            cacheWrapper.Verify(x => x.Get<SimpleResult>(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task ThrowsExceptionWhenNoResultExpectedAndOptionsConfigured()
        {
            // Arrange
            Mock<ICacheKeyProvider> cacheKeyProvider = new Mock<ICacheKeyProvider>();
            Mock<ICommandDispatcher> underlyingCommandDispatcher = new Mock<ICommandDispatcher>();
            Mock<ICacheOptionsProvider> cacheOptionsProvider = new Mock<ICacheOptionsProvider>();
            Mock<ICacheAdapter> cacheWrapper = new Mock<ICacheAdapter>();
            SimpleCommand command = new SimpleCommand();
            CacheOptions options = new CacheOptions(typeof(SimpleCommand), TimeSpan.FromMinutes(5), 1);
            cacheOptionsProvider.Setup(x => x.Get(command)).Returns(options);

            CachedCommandDispatcher dispatcher = new CachedCommandDispatcher(cacheKeyProvider.Object, underlyingCommandDispatcher.Object, cacheOptionsProvider.Object, cacheWrapper.Object);

            // Act and assert
            await Assert.ThrowsAsync< CacheConfigurationException>(() => dispatcher.DispatchAsync(command));
        }

        [Fact]
        public async Task ThrowsExceptionWhenNoCacheKeyReturned()
        {
            // Arrange
            Mock<ICacheKeyProvider> cacheKeyProvider = new Mock<ICacheKeyProvider>();
            Mock<ICommandDispatcher> underlyingCommandDispatcher = new Mock<ICommandDispatcher>();
            Mock<ICacheOptionsProvider> cacheOptionsProvider = new Mock<ICacheOptionsProvider>();
            Mock<ICacheAdapter> cacheWrapper = new Mock<ICacheAdapter>();
            SimpleCommand command = new SimpleCommand();
            CacheOptions options = new CacheOptions(typeof(SimpleCommand), TimeSpan.FromMinutes(5), 1);
            cacheOptionsProvider.Setup(x => x.Get(command)).Returns(options);

            CachedCommandDispatcher dispatcher = new CachedCommandDispatcher(cacheKeyProvider.Object, underlyingCommandDispatcher.Object, cacheOptionsProvider.Object, cacheWrapper.Object);

            // Act and assert
            await Assert.ThrowsAsync<CacheKeyException>(() => dispatcher.DispatchAsync<SimpleCommand,SimpleResult>(command));
        }
    }
}

