using System;
using AccidentalFish.Commanding.Cache.Implementation;
using AccidentalFish.Commanding.Cache.Tests.Unit.TestModel;
using Xunit;

namespace AccidentalFish.Commanding.Cache.Tests.Unit.Implementation
{
    public class PropertyCacheKeyProviderCompilerTests
    {
        [Fact]
        public void ReturnsCompiledFunc()
        {
            // Arrange
            IPropertyCacheKeyProviderCompiler subject = new PropertyCacheKeyProviderCompiler();

            // Act
            Func<SimpleCommand, string> func = subject.Compile<SimpleCommand>();

            // Assert
            Assert.NotNull(func);
        }

        [Fact]
        public void ReturnedFuncCreatesStringFromProperties()
        {
            // Arrange
            IPropertyCacheKeyProviderCompiler subject = new PropertyCacheKeyProviderCompiler();
            Func<SimpleCommand, string> func = subject.Compile<SimpleCommand>();
            string expectedResult = "SimpleCommand|AnotherValue:1|SomeValue:2".GetHashCode().ToString();

            // Act
            string result = func(new SimpleCommand {AnotherValue = 1, SomeValue = 2});

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void ReturnedFuncCreatesSameStringFromTwoCommandsWithSameProperties()
        {
            // Arrange
            IPropertyCacheKeyProviderCompiler subject = new PropertyCacheKeyProviderCompiler();
            Func<SimpleCommand, string> func = subject.Compile<SimpleCommand>();

            // Act
            string result1 = func(new SimpleCommand { AnotherValue = 1, SomeValue = 2 });
            string result2 = func(new SimpleCommand { AnotherValue = 1, SomeValue = 2 });

            // Assert
            Assert.Equal(result1, result2);
        }

        [Fact]
        public void ReturnedFuncCreatesDifferentStringFromTwoCommandsWithSameProperties()
        {
            // Arrange
            IPropertyCacheKeyProviderCompiler subject = new PropertyCacheKeyProviderCompiler();
            Func<SimpleCommand, string> func = subject.Compile<SimpleCommand>();

            // Act
            string result1 = func(new SimpleCommand { AnotherValue = 1, SomeValue = 2 });
            string result2 = func(new SimpleCommand { AnotherValue = 2, SomeValue = 2 });

            // Assert
            Assert.NotEqual(result1, result2);
        }

        [Fact]
        public void ReturnedFuncCreatesDifferentStringFromTwoCommandsWithSamePropertiesDifferentTypes()
        {
            // Arrange
            IPropertyCacheKeyProviderCompiler subject = new PropertyCacheKeyProviderCompiler();
            Func<SimpleCommand, string> func1 = subject.Compile<SimpleCommand>();
            Func<SimpleCommand2, string> func2 = subject.Compile<SimpleCommand2>();

            // Act
            string result1 = func1(new SimpleCommand { AnotherValue = 1, SomeValue = 2 });
            string result2 = func2(new SimpleCommand2 { AnotherValue = 1, SomeValue = 2 });

            // Assert
            Assert.NotEqual(result1, result2);
        }
    }
}
