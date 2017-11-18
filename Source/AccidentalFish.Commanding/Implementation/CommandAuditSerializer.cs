using AccidentalFish.Commanding.Abstractions;
using Newtonsoft.Json;

namespace AccidentalFish.Commanding.Implementation
{
    class CommandAuditSerializer : ICommandAuditSerializer
    {
        public string Serialize(ICommand command)
        {
            return JsonConvert.SerializeObject(command);
        }
    }
}
