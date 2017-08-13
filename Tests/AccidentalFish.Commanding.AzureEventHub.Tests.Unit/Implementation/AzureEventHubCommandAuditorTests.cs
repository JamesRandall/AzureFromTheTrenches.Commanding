using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AccidentalFish.Commanding.AzureEventHub.Implementation;
using AccidentalFish.Commanding.Model;
using Moq;
using Xunit;

namespace AccidentalFish.Commanding.AzureEventHub.Tests.Unit.Implementation
{
    public class AzureEventHubCommandAuditorTests
    {
        [Fact]
        public async Task AuditsWithoutPartitionKey()
        {
            // Arrange
            Mock<IEventHubClient> eventHubClient = new Mock<IEventHubClient>();
            Mock<IEventHubSerializer> eventHubSerializer = new Mock<IEventHubSerializer>();
            Mock<IPartitionKeyProvider> partitionKeyProvider = new Mock<IPartitionKeyProvider>();
            eventHubSerializer.Setup(x => x.Serialize(It.IsAny<AuditItem>())).Returns("a message");
            ICommandAuditor commandAuditor = new AzureEventHubCommandAuditor(eventHubClient.Object, eventHubSerializer.Object, partitionKeyProvider.Object);

            // Act
            await commandAuditor.Audit(new AuditItem());

            // Assert
            eventHubClient.Verify(x => x.SendAsync("a message"));
        }

        [Fact]
        public async Task AuditsWithPartitionKey()
        {
            // Arrange
            Mock<IEventHubClient> eventHubClient = new Mock<IEventHubClient>();
            Mock<IEventHubSerializer> eventHubSerializer = new Mock<IEventHubSerializer>();
            Mock<IPartitionKeyProvider> partitionKeyProvider = new Mock<IPartitionKeyProvider>();
            eventHubSerializer.Setup(x => x.Serialize(It.IsAny<AuditItem>())).Returns("a message");
            partitionKeyProvider.Setup(x => x.GetPartitionKey(It.IsAny<AuditItem>())).Returns("pkey");
            ICommandAuditor commandAuditor = new AzureEventHubCommandAuditor(eventHubClient.Object, eventHubSerializer.Object, partitionKeyProvider.Object);

            // Act
            await commandAuditor.Audit(new AuditItem());

            // Assert
            eventHubClient.Verify(x => x.SendAsync("a message", "pkey"));
        }
    }
}
