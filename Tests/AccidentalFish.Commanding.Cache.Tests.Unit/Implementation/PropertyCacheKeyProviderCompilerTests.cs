using System;
using AzureFromTheTrenches.Commanding.Cache.Implementation;
using AzureFromTheTrenches.Commanding.Cache.Tests.Unit.TestModel;
using Moq;
using Xunit;

namespace AzureFromTheTrenches.Commanding.Cache.Tests.Unit.Implementation
{
    public class PropertyCacheKeyProviderCompilerTests
    {
        [Fact]
        public void ReturnsCompiledFunc()
        {
            // Arrange
            Mock<ICacheKeyHash> cacheKeyHash = new Mock<ICacheKeyHash>();
            cacheKeyHash.Setup(x => x.GetHash(It.IsAny<string>())).Returns("ahash");
            IPropertyCacheKeyProviderCompiler subject = new PropertyCacheKeyProviderCompiler();

            // Act
            Func<SimpleCommand, string> func = subject.Compile<SimpleCommand>(cacheKeyHash.Object);

            // Assert
            Assert.NotNull(func);
        }

        [Fact]
        public void ReturnedFuncCreatesStringFromProperties()
        {
            // Arrange
            IPropertyCacheKeyProviderCompiler subject = new PropertyCacheKeyProviderCompiler();
            Mock<ICacheKeyHash> cacheKeyHash = new Mock<ICacheKeyHash>();
            cacheKeyHash.Setup(x => x.GetHash(It.IsAny<string>())).Returns("ahash");
            Func<SimpleCommand, string> func = subject.Compile<SimpleCommand>(cacheKeyHash.Object);
            
            // Act
            string result = func(new SimpleCommand {AnotherValue = 1, SomeValue = 2});

            // Assert
            Assert.Equal("ahash", result);
        }
    }
}
