using AzureFromTheTrenches.Commanding.Abstractions;
using Newtonsoft.Json;

namespace AzureFromTheTrenches.Commanding.Implementation
{
    class CommandAuditSerializer : ICommandAuditSerializer
    {
        public string Serialize(ICommand command)
        {
            return JsonConvert.SerializeObject(command);
        }
    }
}
