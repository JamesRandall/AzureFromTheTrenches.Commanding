using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.AzureFunctions
{
    public interface ICommandDeserializer
    {
        TCommand Deserialize<TCommand>(string json) where TCommand : ICommand;
    }
}
