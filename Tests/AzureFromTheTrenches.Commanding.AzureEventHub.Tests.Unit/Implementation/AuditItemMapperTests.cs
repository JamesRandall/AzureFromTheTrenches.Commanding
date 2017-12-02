using System;
using System.Collections.Generic;
using AzureFromTheTrenches.Commanding.Abstractions.Model;
using AzureFromTheTrenches.Commanding.AzureEventHub.Implementation;
using Xunit;

namespace AzureFromTheTrenches.Commanding.AzureEventHub.Tests.Unit.Implementation
{
    public class AuditItemMapperTests
    {
        [Fact]
        public void MapsAllProperties()
        {
            // Arrange
            IAuditItemMapper mapper = new AuditItemMapper();
            string commandId = Guid.NewGuid().ToString();
            DateTime dispatchedAt = new DateTime(2017, 12, 8, 11, 12, 30, 234);

            // Act
            var result = mapper.Map(new AuditItem
            {
                AdditionalProperties = new Dictionary<string, string> {{"hello", "world"}},
                CommandId = commandId,
                CommandType = "sometype",
                CorrelationId = "acorrelationid",
                Depth = 2,
                DispatchedUtc = dispatchedAt,
                SerializedCommand = "{'hello':'world'}"
            });

            // Assert
            Assert.Equal("world", result.AdditionalProperties["hello"]);
            Assert.Equal(commandId, result.CommandId);
            Assert.Equal("sometype", result.CommandType);
            Assert.Equal("acorrelationid", result.CorrelationId);
            Assert.Equal(2, result.Depth);
            Assert.Equal(dispatchedAt, result.DispatchedUtc);
            Assert.Equal("{'hello':'world'}", result.Command.Value);
        }
    }
}
