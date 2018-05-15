using AzureFromTheTrenches.Commanding.Abstractions;
using Newtonsoft.Json;

namespace AzureFromTheTrenches.Commanding.AzureFunctions.Infrastructure
{
    internal class CommandDeserializer : ICommandDeserializer
    {
        private readonly JsonSerializerSettings _serializerSettings;

        public CommandDeserializer()
        {
            _serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new JsonSecurityPropertyContractResolver()
            };
        }

        public TCommand Deserialize<TCommand>(string json) where TCommand : ICommand
        {
            return JsonConvert.DeserializeObject<TCommand>(json, _serializerSettings);            
        }
    }
}
