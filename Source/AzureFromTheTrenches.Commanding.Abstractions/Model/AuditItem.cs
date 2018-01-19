using System;
using System.Collections.Generic;

namespace AzureFromTheTrenches.Commanding.Abstractions.Model
{
    public class AuditItem
    {
        public const string PreDispatchType = "predispatch";
        public const string PostDispatchType = "postdispatch";
        public const string ExecutionType = "execution";

        public string Type { get; set; }

        public bool? ExecutedSuccessfully { get; set; }

        public string SerializedCommand { get; set; }

        public string CommandTypeFullName { get; set; }

        public string CommandType { get; set; }

        public string CommandId { get; set; }

        public DateTime DispatchedUtc { get; set; }

        public string CorrelationId { get; set; }

        public int Depth { get; set; }

        public Dictionary<string, string> AdditionalProperties { get; set; }

        public long? ExecutionTimeMs { get; set; }

        public long? DispatchTimeMs { get; set; }
    }
}
