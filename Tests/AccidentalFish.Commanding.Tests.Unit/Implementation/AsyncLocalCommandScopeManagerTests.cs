using System;
using System.Collections.Generic;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Implementation;
using Moq;
using Xunit;

namespace AzureFromTheTrenches.Commanding.Tests.Unit.Implementation
{
    public class AsyncLocalCommandScopeManagerTests
    {
        [Fact]
        public void EnterCreatesInitialContext()
        {
            // Arrange
            Mock<ICommandCorrelationIdProvider> correlationIdProvider = new Mock<ICommandCorrelationIdProvider>();
            Mock<ICommandDispatchContextEnrichment> commandContextEnrichment = new Mock<ICommandDispatchContextEnrichment>();
            correlationIdProvider.Setup(x => x.Create()).Returns("someid");
            AsyncLocalCommandScopeManager manager = new AsyncLocalCommandScopeManager(correlationIdProvider.Object, commandContextEnrichment.Object);

            // Act
            ICommandDispatchContext dispatchContext = manager.Enter();

            // Assert
            Assert.Equal("someid", dispatchContext.CorrelationId);
            Assert.Equal(0, dispatchContext.Depth);
        }

        [Fact]
        public void EnterCreatesInitialContextWithEnrichedProperties()
        {
            // Arrange
            Mock<ICommandCorrelationIdProvider> correlationIdProvider = new Mock<ICommandCorrelationIdProvider>();
            Mock<ICommandDispatchContextEnrichment> commandContextEnrichment = new Mock<ICommandDispatchContextEnrichment>();
            commandContextEnrichment.Setup(x => x.GetAdditionalProperties()).Returns(new Dictionary<string, object>
            {
                {"MyProperty", 25}
            });
            correlationIdProvider.Setup(x => x.Create()).Returns("someid");
            AsyncLocalCommandScopeManager manager = new AsyncLocalCommandScopeManager(correlationIdProvider.Object, commandContextEnrichment.Object);

            // Act
            ICommandDispatchContext dispatchContext = manager.Enter();

            // Assert
            Assert.Equal("someid", dispatchContext.CorrelationId);
            Assert.Equal(25, dispatchContext.AdditionalProperties["MyProperty"]);
            Assert.Equal(0, dispatchContext.Depth);
        }

        [Fact]
        public void ExitOfInitialContextDecrementsToMinusOne()
        {
            // Arrange
            Mock<ICommandCorrelationIdProvider> correlationIdProvider = new Mock<ICommandCorrelationIdProvider>();
            Mock<ICommandDispatchContextEnrichment> commandContextEnrichment = new Mock<ICommandDispatchContextEnrichment>();
            correlationIdProvider.Setup(x => x.Create()).Returns("someid");
            AsyncLocalCommandScopeManager manager = new AsyncLocalCommandScopeManager(correlationIdProvider.Object, commandContextEnrichment.Object);

            // Act
            ICommandDispatchContext dispatchContext = manager.Enter();
            manager.Exit();

            // Assert
            Assert.Equal(-1, dispatchContext.Depth);
        }

        [Fact]
        public void RepeatedEntryIncreasesDepth()
        {
            // Arrange
            Mock<ICommandCorrelationIdProvider> correlationIdProvider = new Mock<ICommandCorrelationIdProvider>();
            Mock<ICommandDispatchContextEnrichment> commandContextEnrichment = new Mock<ICommandDispatchContextEnrichment>();
            correlationIdProvider.Setup(x => x.Create()).Returns("someid");
            AsyncLocalCommandScopeManager manager = new AsyncLocalCommandScopeManager(correlationIdProvider.Object, commandContextEnrichment.Object);

            // Act
            ICommandDispatchContext dispatchContext = manager.Enter();
            manager.Enter();
            manager.Enter();

            // Assert
            Assert.Equal(2, dispatchContext.Depth);
        }

        [Fact]
        public void RepeatedEntryAndMatchingxitDecrementsToMinusOne()
        {
            // Arrange
            Mock<ICommandCorrelationIdProvider> correlationIdProvider = new Mock<ICommandCorrelationIdProvider>();
            Mock<ICommandDispatchContextEnrichment> commandContextEnrichment = new Mock<ICommandDispatchContextEnrichment>();
            correlationIdProvider.Setup(x => x.Create()).Returns("someid");
            AsyncLocalCommandScopeManager manager = new AsyncLocalCommandScopeManager(correlationIdProvider.Object, commandContextEnrichment.Object);

            // Act
            ICommandDispatchContext dispatchContext = manager.Enter();
            manager.Enter();
            manager.Enter();
            manager.Exit();
            manager.Exit();
            manager.Exit();

            // Assert
            Assert.Equal(-1, dispatchContext.Depth);
        }

        [Fact]
        public void EntryAfterFinalExitCreatesNewContext()
        {
            // Arrange
            Mock<ICommandCorrelationIdProvider> correlationIdProvider = new Mock<ICommandCorrelationIdProvider>();
            Mock<ICommandDispatchContextEnrichment> commandContextEnrichment = new Mock<ICommandDispatchContextEnrichment>();
            correlationIdProvider.Setup(x => x.Create()).Returns(() => Guid.NewGuid().ToString());
            AsyncLocalCommandScopeManager manager = new AsyncLocalCommandScopeManager(correlationIdProvider.Object, commandContextEnrichment.Object);
            
            // Act
            ICommandDispatchContext initialDispatchContext = manager.Enter();
            manager.Exit();
            ICommandDispatchContext secondDispatchContext = manager.Enter();

            // Assert
            Assert.NotEqual(initialDispatchContext, secondDispatchContext);
            Assert.NotEqual(initialDispatchContext.CorrelationId, secondDispatchContext.CorrelationId);
        }

        [Fact]
        public void GetCurrentReturnsCurrentWithoutModification()
        {
            // Arrange
            Mock<ICommandCorrelationIdProvider> correlationIdProvider = new Mock<ICommandCorrelationIdProvider>();
            Mock<ICommandDispatchContextEnrichment> commandContextEnrichment = new Mock<ICommandDispatchContextEnrichment>();
            correlationIdProvider.Setup(x => x.Create()).Returns("someid");
            AsyncLocalCommandScopeManager manager = new AsyncLocalCommandScopeManager(correlationIdProvider.Object, commandContextEnrichment.Object);

            // Act
            manager.Enter();
            ICommandDispatchContext dispatchContext = manager.GetCurrent();

            // Assert
            Assert.Equal("someid", dispatchContext.CorrelationId);
            Assert.Equal(0, dispatchContext.Depth);
        }
    }
}
