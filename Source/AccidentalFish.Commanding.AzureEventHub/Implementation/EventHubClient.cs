using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;

namespace AccidentalFish.Commanding.AzureEventHub.Implementation
{
    class EventHubClient : IEventHubClient
    {
        private readonly Microsoft.Azure.EventHubs.EventHubClient _client;

        public EventHubClient(Microsoft.Azure.EventHubs.EventHubClient client)
        {
            _client = client;
        }
        
        public Task SendAsync(string text, string partitionKey)
        {
            return _client.SendAsync(new EventData(Encoding.UTF8.GetBytes(text)), partitionKey);
        }

        public Task SendAsync(string text)
        {
            return _client.SendAsync(new EventData(Encoding.UTF8.GetBytes(text)));
        }
    }
}
