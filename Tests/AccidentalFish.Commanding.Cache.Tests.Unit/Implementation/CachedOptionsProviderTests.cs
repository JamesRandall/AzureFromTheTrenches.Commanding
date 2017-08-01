using System;
using AccidentalFish.Commanding.Cache.Implementation;
using AccidentalFish.Commanding.Cache.Tests.Unit.TestModel;
using Xunit;

namespace AccidentalFish.Commanding.Cache.Tests.Unit.Implementation
{
    public class CachedOptionsProviderTests
    {
        [Fact]
        public void ReturnsRegisteredType()
        {
            // Arrange
            CacheOptionsProvider optionsProvider = new CacheOptionsProvider(new[] { new CacheOptions<SimpleCommand>(TimeSpan.FromMinutes(5))});

            // Act
            CacheOptions options = optionsProvider.Get(new SimpleCommand());

            // Assert
            Assert.NotNull(options);
            Assert.Equal(TimeSpan.FromMinutes(5), options.LifeTime());
            Assert.Null(options.ExpiresAtUtc);
            Assert.Null(options.MaxConcurrentExecutions);
        }

        [Fact]
        public void ReturnsEvalType()
        {
            // Arrange
            CacheOptionsProvider optionsProvider = new CacheOptionsProvider(new[] { new EvalCacheOptions(c => true, TimeSpan.FromMinutes(5))  });

            // Act
            CacheOptions options = optionsProvider.Get(new SimpleCommand());

            // Assert
            Assert.NotNull(options);
            Assert.IsType<EvalCacheOptions>(options);
            Assert.Equal(TimeSpan.FromMinutes(5), options.LifeTime());
            Assert.Null(options.ExpiresAtUtc);
            Assert.Null(options.MaxConcurrentExecutions);
        }

        [Fact]
        public void ConcreteTypeOverridesEvalType()
        {
            // Arrange
            CacheOptionsProvider optionsProvider = new CacheOptionsProvider(new[]
            {
               (CacheOptions) new EvalCacheOptions(c => true, TimeSpan.FromMinutes(5)),
                new CacheOptions<SimpleCommand>(TimeSpan.FromMinutes(5))
            });

            // Act
            CacheOptions options = optionsProvider.Get(new SimpleCommand());

            // Assert
            Assert.NotNull(options);
            Assert.Equal(TimeSpan.FromMinutes(5), options.LifeTime());
            Assert.Null(options.ExpiresAtUtc);
            Assert.Null(options.MaxConcurrentExecutions);
        }

        [Fact]
        public void NotFoundReturnsNull()
        {
            // Arrange
            CacheOptionsProvider optionsProvider = new CacheOptionsProvider(new[] { new CacheOptions<SimpleCommand2>(TimeSpan.FromMinutes(5)) });

            // Act
            CacheOptions options = optionsProvider.Get(new SimpleCommand());

            // Assert
            Assert.Null(options);
        }
    }
}
