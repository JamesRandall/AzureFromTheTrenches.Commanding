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
            private readonly List<string> _auditItems;

            public FirstAuditor(List<string> auditItems)
            {
                _auditItems = auditItems;
            }

            public Task Audit(AuditItem auditItem, CancellationToken cancellationToken)
            {
                _auditItems.Add("FirstAuditor");
                return Task.FromResult(0);
            }
        }

        private class SecondAuditor : ICommandAuditor
        {
            private readonly List<string> _auditItems;

            public SecondAuditor(List<string> auditItems)
            {
                _auditItems = auditItems;
            }

            public Task Audit(AuditItem auditItem, CancellationToken cancellationToken)
            {
                _auditItems.Add("SecondAuditor");
                return Task.FromResult(0);
            }            
        }

        [Fact]
        public async Task RegisteredAuditorIsCalled()
        {
            // Arrange
            List<string> auditItems = new EditableList<string>();
            Mock<ICommandAuditSerializer> serializer = new Mock<ICommandAuditSerializer>();
            CommandAuditPipeline pipeline = new CommandAuditPipeline(t => new FirstAuditor(auditItems), () => serializer.Object);
            pipeline.RegisterPreDispatchAuditor<FirstAuditor>(true);
            
            // Act
            await pipeline.AuditPreDispatch(new SimpleCommand(), new CommandDispatchContext("someid", new Dictionary<string, object>()), default(CancellationToken));

            // Assert
            Assert.Equal("FirstAuditor", auditItems.Single());
        }

        [Fact]
        public async Task AuditorsCalledInRegistrationOrder()
        {
            // Arrange
            List<string> auditItems = new EditableList<string>();
            Mock<ICommandAuditSerializer> serializer = new Mock<ICommandAuditSerializer>();
            CommandAuditPipeline pipeline = new CommandAuditPipeline(t => t == typeof(FirstAuditor) ? (ICommandAuditor)new FirstAuditor(auditItems) : new SecondAuditor(auditItems), () => serializer.Object);
            pipeline.RegisterPreDispatchAuditor<FirstAuditor>(true);
            pipeline.RegisterPreDispatchAuditor<SecondAuditor>(true);

            // Act
            await pipeline.AuditPreDispatch(new SimpleCommand(), new CommandDispatchContext("someid", new Dictionary<string, object>()), default(CancellationToken));

            // Assert
            Assert.Equal("FirstAuditor", auditItems[0]);
            Assert.Equal("SecondAuditor", auditItems[1]);
            Assert.Equal(2, auditItems.Count);
        }
    }
}
