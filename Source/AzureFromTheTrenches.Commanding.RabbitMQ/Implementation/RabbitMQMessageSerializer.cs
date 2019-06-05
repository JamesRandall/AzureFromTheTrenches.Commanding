using AzureFromTheTrenches.Commanding.Abstractions;
using Newtonsoft.Json;
using System.Text;

namespace AzureFromTheTrenches.Commanding.RabbitMQ.Implementation
{
    public class RabbitMQMessageSerializer : IRabbitMQMessageSerializer
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
