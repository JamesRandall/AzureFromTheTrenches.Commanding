using System;
using System.Collections.Generic;
using AccidentalFish.Commanding.Implementation;
using Xunit;

namespace AccidentalFish.Commanding.Tests.Unit.Implementation
{
    public class CommandContextEnrichmentTests
    {
        [Fact]
        public void EnricherInitialisedAtConstructionAddsProperty()
        {
            // Arrange
            IReadOnlyDictionary<string, object> Enricher(IReadOnlyDictionary<string,object> existing) => new Dictionary<string, object>{{"Hello", "World"}};
            CommandDispatchContextEnrichment subject = new CommandDispatchContextEnrichment(new[] { (Func<IReadOnlyDictionary<string, object>, IReadOnlyDictionary<string, object>>)Enricher });

            // Act
            IReadOnlyDictionary<string, object> result = subject.GetAdditionalProperties();

            // Assert
            Assert.Equal(1, result.Count);
            Assert.Equal("World", result["Hello"]);
        }

        [Fact]
        public void EnricherAddedLaterAddsProperty()
        {
            // Arrange
            IReadOnlyDictionary<string, object> Enricher(IReadOnlyDictionary<string, object> existing) => new Dictionary<string, object> { { "Hello", "World" } };
            CommandDispatchContextEnrichment subject = new CommandDispatchContextEnrichment(new List<ICommandDispatchContextEnricher>());
            subject.AddEnrichers(new[] { (Func<IReadOnlyDictionary<string, object>, IReadOnlyDictionary<string, object>>)Enricher });

            // Act
            IReadOnlyDictionary<string, object> result = subject.GetAdditionalProperties();

            // Assert
            Assert.Equal(1, result.Count);
            Assert.Equal("World", result["Hello"]);
        }

        [Fact]
        public void MultipleDistinctEnrichersCombine()
        {
            // Arrange
            IReadOnlyDictionary<string, object> FirstEnricher(IReadOnlyDictionary<string, object> existing) => new Dictionary<string, object> { { "Hello", "World" } };
            IReadOnlyDictionary<string, object> SecondEnricher(IReadOnlyDictionary<string, object> existing) => new Dictionary<string, object> { { "Rabbit", "Hat" } };
            CommandDispatchContextEnrichment subject = new CommandDispatchContextEnrichment(new[] { (Func<IReadOnlyDictionary<string, object>, IReadOnlyDictionary<string, object>>)FirstEnricher });
            subject.AddEnrichers(new[] { (Func<IReadOnlyDictionary<string, object>, IReadOnlyDictionary<string, object>>)SecondEnricher });

            // Act
            IReadOnlyDictionary<string, object> result = subject.GetAdditionalProperties();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("World", result["Hello"]);
            Assert.Equal("Hat", result["Rabbit"]);
        }

        [Fact]
        public void MultipleCoflictingEnrichersLastEnricherWins()
        {
            // Arrange
            IReadOnlyDictionary<string, object> FirstEnricher(IReadOnlyDictionary<string, object> existing) => new Dictionary<string, object> { { "Hello", "World" } };
            IReadOnlyDictionary<string, object> SecondEnricher(IReadOnlyDictionary<string, object> existing) => new Dictionary<string, object> { { "Hello", "Hat" } };
            CommandDispatchContextEnrichment subject = new CommandDispatchContextEnrichment(new[] { (Func<IReadOnlyDictionary<string, object>, IReadOnlyDictionary<string, object>>)FirstEnricher });
            subject.AddEnrichers(new[] { (Func<IReadOnlyDictionary<string, object>, IReadOnlyDictionary<string, object>>)SecondEnricher });

            // Act
            IReadOnlyDictionary<string, object> result = subject.GetAdditionalProperties();

            // Assert
            Assert.Equal(1, result.Count);
            Assert.Equal("Hat", result["Hello"]);
        }

        [Fact]
        public void SecondEnricherIncludesPropertiesFromFirstEnricher()
        {
            // Arrange
            bool containsHello = false;
            IReadOnlyDictionary<string, object> FirstEnricher(IReadOnlyDictionary<string, object> existing) => new Dictionary<string, object> { { "Hello", "World" } };
            IReadOnlyDictionary<string, object> SecondEnricher(IReadOnlyDictionary<string, object> existing)
            {
                containsHello = existing.ContainsKey("Hello");
                return new Dictionary<string, object> { { "Rabbit", "Hat" } };
            }

            CommandDispatchContextEnrichment subject = new CommandDispatchContextEnrichment(new[]
            {
                (Func<IReadOnlyDictionary<string, object>, IReadOnlyDictionary<string, object>>)FirstEnricher,
                SecondEnricher
            });            

            // Act
            IReadOnlyDictionary<string, object> result = subject.GetAdditionalProperties();

            // Assert
            Assert.True(containsHello);
        }
    }
}
