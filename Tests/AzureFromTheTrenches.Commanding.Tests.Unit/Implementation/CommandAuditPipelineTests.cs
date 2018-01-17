using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;
using AzureFromTheTrenches.Commanding.Implementation;
using AzureFromTheTrenches.Commanding.Model;
using AzureFromTheTrenches.Commanding.Tests.Unit.TestModel;
using Castle.Components.DictionaryAdapter;
using Moq;
using Xunit;

namespace AzureFromTheTrenches.Commanding.Tests.Unit.Implementation
{
    public class CommandAuditPipelineTests
    {
        private class FirstAuditor : ICommandAuditor
        {
            private readonly List<string> _auditors;
            private readonly List<AuditItem> _auditItems;

            public FirstAuditor(List<string> auditors, List<AuditItem> auditItems = null)
            {
                _auditors = auditors;
                _auditItems = auditItems;
            }

            public Task Audit(AuditItem auditItem, CancellationToken cancellationToken)
            {
                _auditors.Add("FirstAuditor");
                _auditItems?.Add(auditItem);
                return Task.FromResult(0);
            }
        }

        private class SecondAuditor : ICommandAuditor
        {
            private readonly List<string> _auditors;

            public SecondAuditor(List<string> auditors)
            {
                _auditors = auditors;
            }

            public Task Audit(AuditItem auditItem, CancellationToken cancellationToken)
            {
                _auditors.Add("SecondAuditor");
                return Task.FromResult(0);
            }            
        }

        #region Pre dispatch

        [Fact]
        public async Task RegisteredPreDispatchAuditorIsCalled()
        {
            // Arrange
            List<string> auditItems = new EditableList<string>();
            Mock<ICommandAuditSerializer> serializer = new Mock<ICommandAuditSerializer>();
            Mock<IAuditItemEnricherPipeline> enricherPipeline = new Mock<IAuditItemEnricherPipeline>();
            CommandAuditPipeline pipeline = new CommandAuditPipeline(t => new FirstAuditor(auditItems),
                () => serializer.Object,
                enricherPipeline.Object);
            pipeline.RegisterPreDispatchAuditor<FirstAuditor>(true);
            
            // Act
            await pipeline.AuditPreDispatch(new SimpleCommand(), new CommandDispatchContext("someid", new Dictionary<string, object>()), default(CancellationToken));

            // Assert
            Assert.Equal("FirstAuditor", auditItems.Single());
        }

        [Fact]
        public async Task RegisteredPreDispatchAuditorIsOnlyCalledForRootWhenConfiguredAsSuch()
        {
            // Arrange
            List<string> auditItems = new EditableList<string>();
            Mock<ICommandAuditSerializer> serializer = new Mock<ICommandAuditSerializer>();
            Mock<IAuditItemEnricherPipeline> enricherPipeline = new Mock<IAuditItemEnricherPipeline>();
            CommandAuditPipeline pipeline = new CommandAuditPipeline(t => new FirstAuditor(auditItems),
                () => serializer.Object,
                enricherPipeline.Object);
            pipeline.RegisterPreDispatchAuditor<FirstAuditor>(true);
            ICommandDispatchContext commandDispatchContext = new CommandDispatchContext("someid", new Dictionary<string, object>());
            commandDispatchContext.Increment();

            // Act
            await pipeline.AuditPreDispatch(new SimpleCommand(), commandDispatchContext, default(CancellationToken));

            // Assert
            Assert.Empty(auditItems);
        }

        [Fact]
        public async Task NoRegisteredPreDispatchAuditorCausesNoError()
        {
            // Arrange
            List<string> auditItems = new EditableList<string>();
            Mock<ICommandAuditSerializer> serializer = new Mock<ICommandAuditSerializer>();
            Mock<IAuditItemEnricherPipeline> enricherPipeline = new Mock<IAuditItemEnricherPipeline>();
            CommandAuditPipeline pipeline = new CommandAuditPipeline(t => new FirstAuditor(auditItems),
                () => serializer.Object,
                enricherPipeline.Object);
            
            // Act
            await pipeline.AuditPreDispatch(new SimpleCommand(), new CommandDispatchContext("someid", new Dictionary<string, object>()), default(CancellationToken));

            // Assert - lack of exception is pass
        }

        [Fact]
        public async Task PreDispatchAuditorPipelineExtractsCommandId()
        {
            // Arrange
            List<string> auditors = new EditableList<string>();
            List<AuditItem> auditItems = new List<AuditItem>();
            Mock<ICommandAuditSerializer> serializer = new Mock<ICommandAuditSerializer>();
            Mock<IAuditItemEnricherPipeline> enricherPipeline = new Mock<IAuditItemEnricherPipeline>();
            CommandAuditPipeline pipeline = new CommandAuditPipeline(t => new FirstAuditor(auditors, auditItems),
                () => serializer.Object,
                enricherPipeline.Object);
            pipeline.RegisterPreDispatchAuditor<FirstAuditor>(true);
            SimpleIdentifiableCommand command = new SimpleIdentifiableCommand
            {
                CommandId = "helloworld"
            };

            // Act
            await pipeline.AuditPreDispatch(command, new CommandDispatchContext("someid", new Dictionary<string, object>()), default(CancellationToken));

            // Assert
            Assert.Equal("helloworld", auditItems.Single().CommandId);
        }

        [Fact]
        public async Task PreDispatchAuditorsCalledInRegistrationOrder()
        {
            // Arrange
            List<string> auditItems = new EditableList<string>();
            Mock<ICommandAuditSerializer> serializer = new Mock<ICommandAuditSerializer>();
            Mock<IAuditItemEnricherPipeline> enricherPipeline = new Mock<IAuditItemEnricherPipeline>();
            CommandAuditPipeline pipeline = new CommandAuditPipeline(t => t == typeof(FirstAuditor) ? (ICommandAuditor)new FirstAuditor(auditItems) : new SecondAuditor(auditItems),
                () => serializer.Object,
                enricherPipeline.Object);
            pipeline.RegisterPreDispatchAuditor<FirstAuditor>(true);
            pipeline.RegisterPreDispatchAuditor<SecondAuditor>(true);

            // Act
            await pipeline.AuditPreDispatch(new SimpleCommand(), new CommandDispatchContext("someid", new Dictionary<string, object>()), default(CancellationToken));

            // Assert
            Assert.Equal("FirstAuditor", auditItems[0]);
            Assert.Equal("SecondAuditor", auditItems[1]);
            Assert.Equal(2, auditItems.Count);
        }

        #endregion

        #region Post dispatch

        [Fact]
        public async Task RegisteredPostDispatchAuditorIsCalled()
        {
            // Arrange
            List<string> auditItems = new EditableList<string>();
            Mock<ICommandAuditSerializer> serializer = new Mock<ICommandAuditSerializer>();
            Mock<IAuditItemEnricherPipeline> enricherPipeline = new Mock<IAuditItemEnricherPipeline>();
            CommandAuditPipeline pipeline = new CommandAuditPipeline(t => new FirstAuditor(auditItems),
                () => serializer.Object,
                enricherPipeline.Object);
            pipeline.RegisterPostDispatchAuditor<FirstAuditor>(true);

            // Act
            await pipeline.AuditPostDispatch(new SimpleCommand(), new CommandDispatchContext("someid", new Dictionary<string, object>()), default(CancellationToken));

            // Assert
            Assert.Equal("FirstAuditor", auditItems.Single());
        }

        [Fact]
        public async Task RegisteredPostDispatchAuditorIsOnlyCalledForRootWhenConfiguredAsSuch()
        {
            // Arrange
            List<string> auditItems = new EditableList<string>();
            Mock<ICommandAuditSerializer> serializer = new Mock<ICommandAuditSerializer>();
            Mock<IAuditItemEnricherPipeline> enricherPipeline = new Mock<IAuditItemEnricherPipeline>();
            CommandAuditPipeline pipeline = new CommandAuditPipeline(t => new FirstAuditor(auditItems),
                () => serializer.Object,
                enricherPipeline.Object);
            pipeline.RegisterPostDispatchAuditor<FirstAuditor>(true);
            ICommandDispatchContext commandDispatchContext = new CommandDispatchContext("someid", new Dictionary<string, object>());
            commandDispatchContext.Increment();

            // Act
            await pipeline.AuditPostDispatch(new SimpleCommand(), commandDispatchContext, default(CancellationToken));

            // Assert
            Assert.Empty(auditItems);
        }

        [Fact]
        public async Task NoRegisteredPostDispatchAuditorCausesNoError()
        {
            // Arrange
            List<string> auditItems = new EditableList<string>();
            Mock<ICommandAuditSerializer> serializer = new Mock<ICommandAuditSerializer>();
            Mock<IAuditItemEnricherPipeline> enricherPipeline = new Mock<IAuditItemEnricherPipeline>();
            CommandAuditPipeline pipeline = new CommandAuditPipeline(t => new FirstAuditor(auditItems),
                () => serializer.Object,
                enricherPipeline.Object);

            // Act
            await pipeline.AuditPostDispatch(new SimpleCommand(), new CommandDispatchContext("someid", new Dictionary<string, object>()), default(CancellationToken));

            // Assert - lack of exception is pass
        }

        [Fact]
        public async Task PostDispatchAuditorPipelineExtractsCommandId()
        {
            // Arrange
            List<string> auditors = new EditableList<string>();
            List<AuditItem> auditItems = new List<AuditItem>();
            Mock<ICommandAuditSerializer> serializer = new Mock<ICommandAuditSerializer>();
            Mock<IAuditItemEnricherPipeline> enricherPipeline = new Mock<IAuditItemEnricherPipeline>();
            CommandAuditPipeline pipeline = new CommandAuditPipeline(t => new FirstAuditor(auditors, auditItems),
                () => serializer.Object,
                enricherPipeline.Object);
            pipeline.RegisterPostDispatchAuditor<FirstAuditor>(true);
            SimpleIdentifiableCommand command = new SimpleIdentifiableCommand
            {
                CommandId = "helloworld"
            };

            // Act
            await pipeline.AuditPostDispatch(command, new CommandDispatchContext("someid", new Dictionary<string, object>()), default(CancellationToken));

            // Assert
            Assert.Equal("helloworld", auditItems.Single().CommandId);
        }

        [Fact]
        public async Task PostDispatchAuditorsCalledInRegistrationOrder()
        {
            // Arrange
            List<string> auditItems = new EditableList<string>();
            Mock<ICommandAuditSerializer> serializer = new Mock<ICommandAuditSerializer>();
            Mock<IAuditItemEnricherPipeline> enricherPipeline = new Mock<IAuditItemEnricherPipeline>();
            CommandAuditPipeline pipeline = new CommandAuditPipeline(t => t == typeof(FirstAuditor) ? (ICommandAuditor)new FirstAuditor(auditItems) : new SecondAuditor(auditItems),
                () => serializer.Object,
                enricherPipeline.Object);
            pipeline.RegisterPostDispatchAuditor<FirstAuditor>(true);
            pipeline.RegisterPostDispatchAuditor<SecondAuditor>(true);

            // Act
            await pipeline.AuditPostDispatch(new SimpleCommand(), new CommandDispatchContext("someid", new Dictionary<string, object>()), default(CancellationToken));

            // Assert
            Assert.Equal("FirstAuditor", auditItems[0]);
            Assert.Equal("SecondAuditor", auditItems[1]);
            Assert.Equal(2, auditItems.Count);
        }

        #endregion

        #region Post dispatch

        [Fact]
        public async Task RegisteredExecutionAuditorIsCalled()
        {
            // Arrange
            List<string> auditItems = new EditableList<string>();
            Mock<ICommandAuditSerializer> serializer = new Mock<ICommandAuditSerializer>();
            Mock<IAuditItemEnricherPipeline> enricherPipeline = new Mock<IAuditItemEnricherPipeline>();
            CommandAuditPipeline pipeline = new CommandAuditPipeline(t => new FirstAuditor(auditItems),
                () => serializer.Object,
                enricherPipeline.Object);
            pipeline.RegisterExecutionAuditor<FirstAuditor>(true);

            // Act
            await pipeline.AuditExecution(new SimpleCommand(), new CommandDispatchContext("someid", new Dictionary<string, object>()), true, default(CancellationToken));

            // Assert
            Assert.Equal("FirstAuditor", auditItems.Single());
        }

        [Fact]
        public async Task RegisteredExecutionAuditorIsOnlyCalledForRootWhenConfiguredAsSuch()
        {
            // Arrange
            List<string> auditItems = new EditableList<string>();
            Mock<ICommandAuditSerializer> serializer = new Mock<ICommandAuditSerializer>();
            Mock<IAuditItemEnricherPipeline> enricherPipeline = new Mock<IAuditItemEnricherPipeline>();
            CommandAuditPipeline pipeline = new CommandAuditPipeline(t => new FirstAuditor(auditItems),
                () => serializer.Object,
                enricherPipeline.Object);
            pipeline.RegisterExecutionAuditor<FirstAuditor>(true);
            ICommandDispatchContext commandDispatchContext = new CommandDispatchContext("someid", new Dictionary<string, object>());
            commandDispatchContext.Increment();

            // Act
            await pipeline.AuditExecution(new SimpleCommand(), commandDispatchContext, true, default(CancellationToken));

            // Assert
            Assert.Empty(auditItems);
        }

        [Fact]
        public async Task NoRegisteredExecutionAuditorCausesNoError()
        {
            // Arrange
            List<string> auditItems = new EditableList<string>();
            Mock<ICommandAuditSerializer> serializer = new Mock<ICommandAuditSerializer>();
            Mock<IAuditItemEnricherPipeline> enricherPipeline = new Mock<IAuditItemEnricherPipeline>();
            CommandAuditPipeline pipeline = new CommandAuditPipeline(t => new FirstAuditor(auditItems),
                () => serializer.Object,
                enricherPipeline.Object);

            // Act
            await pipeline.AuditExecution(new SimpleCommand(), new CommandDispatchContext("someid", new Dictionary<string, object>()), true, default(CancellationToken));

            // Assert - lack of exception is pass
        }

        [Fact]
        public async Task ExecutionAuditorPipelineExtractsCommandId()
        {
            // Arrange
            List<string> auditors = new EditableList<string>();
            List<AuditItem> auditItems = new List<AuditItem>();
            Mock<ICommandAuditSerializer> serializer = new Mock<ICommandAuditSerializer>();
            Mock<IAuditItemEnricherPipeline> enricherPipeline = new Mock<IAuditItemEnricherPipeline>();
            CommandAuditPipeline pipeline = new CommandAuditPipeline(t => new FirstAuditor(auditors, auditItems),
                () => serializer.Object,
                enricherPipeline.Object);
            pipeline.RegisterExecutionAuditor<FirstAuditor>(true);
            SimpleIdentifiableCommand command = new SimpleIdentifiableCommand
            {
                CommandId = "helloworld"
            };

            // Act
            await pipeline.AuditExecution(command, new CommandDispatchContext("someid", new Dictionary<string, object>()), true, default(CancellationToken));

            // Assert
            Assert.Equal("helloworld", auditItems.Single().CommandId);
        }

        [Fact]
        public async Task ExecutionAuditorsCalledInRegistrationOrder()
        {
            // Arrange
            List<string> auditItems = new EditableList<string>();
            Mock<ICommandAuditSerializer> serializer = new Mock<ICommandAuditSerializer>();
            Mock<IAuditItemEnricherPipeline> enricherPipeline = new Mock<IAuditItemEnricherPipeline>();
            CommandAuditPipeline pipeline = new CommandAuditPipeline(t => t == typeof(FirstAuditor) ? (ICommandAuditor)new FirstAuditor(auditItems) : new SecondAuditor(auditItems),
                () => serializer.Object,
                enricherPipeline.Object);
            pipeline.RegisterExecutionAuditor<FirstAuditor>(true);
            pipeline.RegisterExecutionAuditor<SecondAuditor>(true);

            // Act
            await pipeline.AuditExecution(new SimpleCommand(), new CommandDispatchContext("someid", new Dictionary<string, object>()), true, default(CancellationToken));

            // Assert
            Assert.Equal("FirstAuditor", auditItems[0]);
            Assert.Equal("SecondAuditor", auditItems[1]);
            Assert.Equal(2, auditItems.Count);
        }

        #endregion
    }
}
