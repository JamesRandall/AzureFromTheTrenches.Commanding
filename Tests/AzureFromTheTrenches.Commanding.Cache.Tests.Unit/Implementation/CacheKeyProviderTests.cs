using AzureFromTheTrenches.Commanding.Cache.Implementation;
using AzureFromTheTrenches.Commanding.Cache.Tests.Unit.TestModel;
using Moq;
using Xunit;

namespace AzureFromTheTrenches.Commanding.Cache.Tests.Unit.Implementation
{
    public class PropertyCacheKeyProviderTests
    {
        [Fact]
        public void CreatesFuncWhenNotInCache()
        {
            // Arrange
            Mock<IPropertyCacheKeyProviderCompiler> compiler = new Mock<IPropertyCacheKeyProviderCompiler>();
            Mock<ICacheKeyHash> cacheKeyHash = new Mock<ICacheKeyHash>();
            compiler.Setup(x => x.Compile<SimpleCommand>(It.IsAny<ICacheKeyHash>())).Returns(c => "hello");
            SimpleCommand command = new SimpleCommand
            {
                SomeValue = 1
            };            
            ICacheKeyProvider subject = new PropertyCacheKeyProvider(compiler.Object, cacheKeyHash.Object);

            // Act
            subject.CacheKey(command);

            // Assert
            compiler.Verify(x => x.Compile<SimpleCommand>(cacheKeyHash.Object), Times.Once);
        }

        [Fact]
        public void UsesCacheWhenCommandTypeReused()
        {
            // Arrange
            Mock<IPropertyCacheKeyProviderCompiler> compiler = new Mock<IPropertyCacheKeyProviderCompiler>();
            Mock<ICacheKeyHash> cacheKeyHash = new Mock<ICacheKeyHash>();
            compiler.Setup(x => x.Compile<SimpleCommand>(It.IsAny<ICacheKeyHash>())).Returns(c => "hello");
            SimpleCommand command = new SimpleCommand
            {
                SomeValue = 1
            };
            SimpleCommand command2 = new SimpleCommand
            {
                SomeValue = 1
            };
            ICacheKeyProvider subject = new PropertyCacheKeyProvider(compiler.Object, cacheKeyHash.Object);

            // Act
            subject.CacheKey(command);
            subject.CacheKey(command);

            // Assert
            compiler.Verify(x => x.Compile<SimpleCommand>(cacheKeyHash.Object), Times.Once);
        }

        [Fact]
        public void CacheIsKeyedOnType()
        {
            // Arrange
            Mock<IPropertyCacheKeyProviderCompiler> compiler = new Mock<IPropertyCacheKeyProviderCompiler>();
            Mock<ICacheKeyHash> cacheKeyHash = new Mock<ICacheKeyHash>();
            compiler.Setup(x => x.Compile<SimpleCommand>(It.IsAny<ICacheKeyHash>())).Returns(c => "hello");
            compiler.Setup(x => x.Compile<SimpleCommand2>(It.IsAny<ICacheKeyHash>())).Returns(c => "hello");
            SimpleCommand command = new SimpleCommand
            {
                SomeValue = 1
            };
            SimpleCommand2 command2 = new SimpleCommand2
            {
                SomeValue = 1
            };
            ICacheKeyProvider subject = new PropertyCacheKeyProvider(compiler.Object, cacheKeyHash.Object);

            // Act
            subject.CacheKey(command);
            subject.CacheKey(command2);

            // Assert
            compiler.Verify(x => x.Compile<SimpleCommand>(cacheKeyHash.Object), Times.Exactly(1));
            compiler.Verify(x => x.Compile<SimpleCommand2>(cacheKeyHash.Object), Times.Exactly(1));
        }
    }
}
