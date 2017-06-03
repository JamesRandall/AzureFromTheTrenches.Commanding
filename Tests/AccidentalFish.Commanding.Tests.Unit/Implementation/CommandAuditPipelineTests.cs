using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccidentalFish.Commanding.Implementation;
using AccidentalFish.Commanding.Model;
using AccidentalFish.Commanding.Tests.Unit.TestModel;
using Castle.Components.DictionaryAdapter;
using Moq;
using Xunit;

namespace AccidentalFish.Commanding.Tests.Unit.Implementation
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

            public Task Audit(AuditItem auditItem)
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

            public Task Audit(AuditItem auditItem)
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
            pipeline.RegisterAuditor<FirstAuditor>();
            Guid commandId = Guid.NewGuid();

            // Act
            await pipeline.Audit(new SimpleCommand(), commandId, new CommandDispatchContext("someid", new Dictionary<string, object>()));

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
            pipeline.RegisterAuditor<FirstAuditor>();
            pipeline.RegisterAuditor<SecondAuditor>();
            Guid commandId = Guid.NewGuid();

            // Act
            await pipeline.Audit(new SimpleCommand(), commandId, new CommandDispatchContext("someid", new Dictionary<string, object>()));

            // Assert
            Assert.Equal("FirstAuditor", auditItems[0]);
            Assert.Equal("SecondAuditor", auditItems[1]);
            Assert.Equal(2, auditItems.Count);
        }
    }
}
