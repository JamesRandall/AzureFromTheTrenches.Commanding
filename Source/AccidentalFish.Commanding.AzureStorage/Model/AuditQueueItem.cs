using System;
using System.Collections.Generic;

namespace AccidentalFish.Commanding.AzureStorage.Model
{
    public class AuditQueueItem
    {
        public DateTime RecordedAtUtc { get; set; }

        public string CommandType { get; set; }

        public string CorrelationId { get; set; }

        public int Depth { get; set; }

        public Guid CommandId { get; set; }
        
        public Dictionary<string, string> AdditionalProperties { get; set; }

        public string CommandPayloadJson { get; set; }
    }
}
