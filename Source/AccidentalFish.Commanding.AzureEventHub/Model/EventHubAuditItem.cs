using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace AccidentalFish.Commanding.AzureEventHub.Model
{
    public class EventHubAuditItem
    {
        public JRaw Command { get; set; }

        public String CommandType { get; set; }

        public Guid CommandId { get; set; }

        public DateTime DispatchedUtc { get; set; }

        public string CorrelationId { get; set; }

        public int Depth { get; set; }

        public Dictionary<string, string> AdditionalProperties { get; set; }
    }
}
