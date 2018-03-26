using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.AzureServiceBus
{
    public interface IServiceBusMessageSerializer
    {
        byte[] Serialize(ICommand command);
        TCommand Deserialize<TCommand>(byte[] serializedCommand) where TCommand : ICommand;
    }
}
