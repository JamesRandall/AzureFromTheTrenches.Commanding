using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccidentalFish.Commanding.Implementation;
using AccidentalFish.Commanding.Model;
using AccidentalFish.Commanding.Tests.Unit.TestModel;
using Castle.Components.DictionaryAdapter;
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

            public Task AuditWithCommandPayload<TCommand>(TCommand command, ICommandDispatchContext dispatchContext) where TCommand : class
            {
                _auditItems.Add("FirstAuditor");
                return Task.FromResult(0);
            }

            public Task AuditWithNoPayload(Guid commandId, string commandType, ICommandDispatchContext dispatchContext)
            {
                throw new NotImplementedException();
            }
        }

        private class SecondAuditor : ICommandAuditor
        {
            private readonly List<string> _auditItems;

            public SecondAuditor(List<string> auditItems)
            {
                _auditItems = auditItems;
            }

            public Task AuditWithCommandPayload<TCommand>(TCommand command, ICommandDispatchContext dispatchContext) where TCommand : class
            {
                _auditItems.Add("SecondAuditor");
                return Task.FromResult(0);
            }

            public Task AuditWithNoPayload(Guid commandId, string commandType, ICommandDispatchContext dispatchContext)
            {
                throw new NotImplementedException();
            }
        }

        [Fact]
        public async Task RegisteredAuditorIsCalled()
        {
            // Arrange
            List<string> auditItems = new EditableList<string>();
            CommandAuditPipeline pipeline = new CommandAuditPipeline(t => new FirstAuditor(auditItems));
            pipeline.RegisterAuditor<FirstAuditor>();

            // Act
            await pipeline.Audit(new SimpleCommand(), new CommandDispatchContext("someid", new Dictionary<string, object>()));

            // Assert
            Assert.Equal("FirstAuditor", auditItems.Single());
        }

        [Fact]
        public async Task AuditorsCalledInRegistrationOrder()
        {
            // Arrange
            List<string> auditItems = new EditableList<string>();
            CommandAuditPipeline pipeline = new CommandAuditPipeline(t => t == typeof(FirstAuditor) ? (ICommandAuditor)new FirstAuditor(auditItems) : new SecondAuditor(auditItems));
            pipeline.RegisterAuditor<FirstAuditor>();
            pipeline.RegisterAuditor<SecondAuditor>();

            // Act
            await pipeline.Audit(new SimpleCommand(), new CommandDispatchContext("someid", new Dictionary<string, object>()));

            // Assert
            Assert.Equal("FirstAuditor", auditItems[0]);
            Assert.Equal("SecondAuditor", auditItems[1]);
            Assert.Equal(2, auditItems.Count);
        }
    }
}
