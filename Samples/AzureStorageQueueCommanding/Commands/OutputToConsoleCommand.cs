using AccidentalFish.Commanding.Abstractions;
using AccidentalFish.Commanding.Abstractions.Model;

namespace AzureStorageQueueCommanding.Commands
{
    public class OutputToConsoleCommand : ICommand<DeferredCommandResult>
    {
        public string Message { get; set; }
    }
}
