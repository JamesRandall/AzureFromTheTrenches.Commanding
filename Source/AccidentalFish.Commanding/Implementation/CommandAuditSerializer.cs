using Newtonsoft.Json;

namespace AccidentalFish.Commanding.Implementation
{
    class CommandAuditSerializer : ICommandAuditSerializer
    {
        public string Serialize<TCommand>(TCommand command) where TCommand : class
        {
            return JsonConvert.SerializeObject(command);
        }
    }
}
