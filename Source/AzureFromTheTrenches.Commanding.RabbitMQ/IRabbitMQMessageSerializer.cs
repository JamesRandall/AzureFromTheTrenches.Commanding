using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.RabbitMQ
{
    public interface IRabbitMQMessageSerializer
    {
        byte[] Serialize(ICommand command);
        TCommand Deserialize<TCommand>(byte[] serializedCommand) where TCommand : ICommand;
    }
}
