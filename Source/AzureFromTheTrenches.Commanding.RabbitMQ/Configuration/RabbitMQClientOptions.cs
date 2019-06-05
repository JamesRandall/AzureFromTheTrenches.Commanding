using System.Collections.Generic;

namespace AzureFromTheTrenches.Commanding.RabbitMQ.Configuration
{
    public class RabbitMQClientOptions
    {
        public string HostName { get; set; } = "localhost";

        public int Port { get; set; } = 5672;

        public string UserName { get; set; } = "guest";

        public string Password { get; set; } = "guest";

        public string VirtualHost { get; set; } = "/";

        public string Queue { get; set; } = "TestQueue";

        public bool AutomaticRecoveryEnabled { get; set; } = true;

        public bool TopologyRecoveryEnabled { get; set; } = true;

        public int RequestedConnectionTimeout { get; set; } = 60000;

        public ushort RequestedHeartbeat { get; set; } = 60;

        public bool Durable { get; set; } = false;

        public bool Exclusive { get; set; } = false;

        public bool AutoDelete { get; set; } = false;

        public IDictionary<string, object> QueueArguments { get; set; } = null;
    }
}