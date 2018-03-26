using System.Text;
using AzureFromTheTrenches.Commanding.Abstractions;
using Newtonsoft.Json;

namespace AzureFromTheTrenches.Commanding.AzureServiceBus.Implementation
{
    internal class JsonServiceBusMessageSerializer : IServiceBusMessageSerializer
    {
        public byte[] Serialize(ICommand command)
        {
            string json = JsonConvert.SerializeObject(command);
            return Encoding.UTF8.GetBytes(json);
        }

        public TCommand Deserialize<TCommand>(byte[] serializedCommand) where TCommand : ICommand
        {
            string json = Encoding.UTF8.GetString(serializedCommand);
            return JsonConvert.DeserializeObject<TCommand>(json);
        }
    }
}
