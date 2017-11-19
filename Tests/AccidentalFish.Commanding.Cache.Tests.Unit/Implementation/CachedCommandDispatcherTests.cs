using System;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;
using AzureFromTheTrenches.Commanding.Cache.Implementation;
using AzureFromTheTrenches.Commanding.Cache.Tests.Unit.TestModel;
using Moq;
using Xunit;

namespace AzureFromTheTrenches.Commanding.Cache.Tests.Unit.Implementation
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
            await dispatcher.DispatchAsync(command);

            // Assert
            underlyingCommandDispatcher.Verify(x => x.DispatchAsync(command));
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
            cacheOptionsProvider.Setup(x => x.Get(It.IsAny<ICommand>())).Returns(options);
            cacheKeyProvider.Setup(x => x.CacheKey(It.IsAny<ICommand>())).Returns("akey");

            CachedCommandDispatcher dispatcher = new CachedCommandDispatcher(cacheKeyProvider.Object, underlyingCommandDispatcher.Object, cacheOptionsProvider.Object, cacheWrapper.Object);

            // Act
            var result = await dispatcher.DispatchAsync(command);

            // Assert
            underlyingCommandDispatcher.Verify(x => x.DispatchAsync(command), Times.Never);
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
            cacheOptionsProvider.Setup(x => x.Get(It.IsAny<ICommand>())).Returns(options);
            cacheKeyProvider.Setup(x => x.CacheKey(It.IsAny<ICommand>())).Returns("akey");
            cacheWrapper.Setup(x => x.Set("akey", It.IsAny<object>(), TimeSpan.FromMinutes(5))).Returns(Task.FromResult(0));
            underlyingCommandDispatcher.Setup(x => x.DispatchAsync(command)).ReturnsAsync(new CommandResult<SimpleResult>(cachedResult, false));

            CachedCommandDispatcher dispatcher = new CachedCommandDispatcher(cacheKeyProvider.Object, underlyingCommandDispatcher.Object, cacheOptionsProvider.Object, cacheWrapper.Object);

            // Act
            var result = await dispatcher.DispatchAsync(command);

            // Assert
            underlyingCommandDispatcher.Verify(x => x.DispatchAsync(command), Times.Once, "Underlying dispatched was not called");
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
            cacheOptionsProvider.Setup(x => x.Get(It.IsAny<ICommand>())).Returns(options);
            cacheKeyProvider.Setup(x => x.CacheKey(It.IsAny<ICommand>())).Returns("akey");
            cacheWrapper.Setup(x => x.Set("akey", It.IsAny<object>(), TimeSpan.FromMinutes(5))).Returns(Task.FromResult(0));
            underlyingCommandDispatcher.Setup(x => x.DispatchAsync(command)).ReturnsAsync(new CommandResult<SimpleResult>(cachedResult, false));

            CachedCommandDispatcher dispatcher = new CachedCommandDispatcher(cacheKeyProvider.Object, underlyingCommandDispatcher.Object, cacheOptionsProvider.Object, cacheWrapper.Object);

            // Act
            var result = await dispatcher.DispatchAsync(command);

            // Assert
            underlyingCommandDispatcher.Verify(x => x.DispatchAsync(command), Times.Once, "Underlying dispatched was not called");
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
        public async Task ThrowsExceptionWhenNoCacheKeyReturned()
        {
            // Arrange
            Mock<ICacheKeyProvider> cacheKeyProvider = new Mock<ICacheKeyProvider>();
            Mock<ICommandDispatcher> underlyingCommandDispatcher = new Mock<ICommandDispatcher>();
            Mock<ICacheOptionsProvider> cacheOptionsProvider = new Mock<ICacheOptionsProvider>();
            Mock<ICacheAdapter> cacheWrapper = new Mock<ICacheAdapter>();
            SimpleCommand command = new SimpleCommand();
            CacheOptions options = new CacheOptions(typeof(SimpleCommand), TimeSpan.FromMinutes(5), 1);
            cacheOptionsProvider.Setup(x => x.Get(It.IsAny<ICommand>())).Returns(options);

            CachedCommandDispatcher dispatcher = new CachedCommandDispatcher(cacheKeyProvider.Object, underlyingCommandDispatcher.Object, cacheOptionsProvider.Object, cacheWrapper.Object);

            // Act and assert
            await Assert.ThrowsAsync<CacheKeyException>(() => dispatcher.DispatchAsync(command));
        }
    }
}

