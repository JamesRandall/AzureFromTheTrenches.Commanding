using AzureFromTheTrenches.Commanding.Cache.Implementation;
using Xunit;

namespace AzureFromTheTrenches.Commanding.Cache.Tests.Unit.Implementation
{
    public class SimpleCacheKeyTests
    {
        [Fact]
        public void ReturnsHashedString()
        {
            // Arrange
            SimpleCacheKeyHash subject = new SimpleCacheKeyHash();

            // Act
            string result = subject.GetHash("somekey");

            // Assert
            Assert.True(result.Length > 0);
        }

        [Fact]
        public void HashingStringTwiceAlwaysReturnsSameHash()
        {
            // Arrange
            SimpleCacheKeyHash subject = new SimpleCacheKeyHash();

            // Act
            string result1 = subject.GetHash("somekey");
            string result2 = subject.GetHash("somekey");

            // Assert
            Assert.Equal(result1, result2);
        }

        [Fact]
        public void DifferentStringsReturnDifferentHash()
        {
            // Arrange
            SimpleCacheKeyHash subject = new SimpleCacheKeyHash();

            // Act
            string result1 = subject.GetHash("Somekey");
            string result2 = subject.GetHash("somekey");

            // Assert
            Assert.NotEqual(result1, result2);
        }
    }
}
