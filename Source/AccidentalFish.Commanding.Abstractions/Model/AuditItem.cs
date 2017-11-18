using System;
using System.Collections.Generic;

namespace AccidentalFish.Commanding.Abstractions.Model
{
    public class AuditItem
    {
        public string SerializedCommand { get; set; }

        public String CommandType { get; set; }

        public Guid CommandId { get; set; }

        public DateTime DispatchedUtc { get; set; }

        public string CorrelationId { get; set; }

        public int Depth { get; set; }

        public Dictionary<string, string> AdditionalProperties { get; set; }
    }
}
