using System;
using System.Collections.Generic;

namespace AzureFromTheTrenches.Commanding.Abstractions.Model
{
    public class AuditItem
    {
        public string Type { get; set; }

        public bool? ExecutedSuccessfully { get; set; }

        public string SerializedCommand { get; set; }

        public String CommandType { get; set; }

        public string CommandId { get; set; }

        public DateTime DispatchedUtc { get; set; }

        public string CorrelationId { get; set; }

        public int Depth { get; set; }

        public Dictionary<string, string> AdditionalProperties { get; set; }
    }
}
