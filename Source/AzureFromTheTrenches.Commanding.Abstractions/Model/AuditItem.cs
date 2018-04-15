using System;
using System.Collections.Generic;

namespace AzureFromTheTrenches.Commanding.Abstractions.Model
{
    /// <summary>
    /// An audit item
    /// </summary>
    public class AuditItem
    {
        /// <summary>
        /// The code used to identify pre dispatch audit items
        /// </summary>
        public const string PreDispatchType = "predispatch";
        /// <summary>
        /// The code used to identify post dispatch audit items
        /// </summary>
        public const string PostDispatchType = "postdispatch";
        /// <summary>
        /// The type used to identify post execution audit items
        /// </summary>
        public const string ExecutionType = "execution";

        /// <summary>
        /// The type of the audit item (pre-dispatch, post-dispatch, execution)
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Was the command executed successfully - will be null in the dispatch stages
        /// </summary>
        public bool? ExecutedSuccessfully { get; set; }

        /// <summary>
        /// A serialized version of the command
        /// </summary>
        public string SerializedCommand { get; set; }

        /// <summary>
        /// The full type name of the command
        /// </summary>
        public string CommandTypeFullName { get; set; }

        /// <summary>
        /// The short type name of the command
        /// </summary>
        public string CommandType { get; set; }

        /// <summary>
        /// The ID of the command
        /// </summary>
        public string CommandId { get; set; }

        /// <summary>
        /// The date and time the command was dispatched
        /// </summary>
        public DateTime DispatchedUtc { get; set; }

        /// <summary>
        /// Correlation ID from the command dispatch context
        /// </summary>
        public string CorrelationId { get; set; }

        /// <summary>
        /// The depth of the command from the dispatch context
        /// </summary>
        public int Depth { get; set; }

        /// <summary>
        /// Any additional properties set by enrichers
        /// </summary>
        public Dictionary<string, string> AdditionalProperties { get; set; }

        /// <summary>
        /// The time taken to execute the command (will be null in dispatch audit items or if metrics are disabled)
        /// </summary>
        public long? ExecutionTimeMs { get; set; }

        /// <summary>
        /// The time taken to dispatch the command (will be null in execution audit items or if metrics are disabled)
        /// </summary>
        public long? DispatchTimeMs { get; set; }
    }
}
