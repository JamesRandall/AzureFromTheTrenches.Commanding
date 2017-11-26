using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;

namespace AzureEventHubAuditing.Commands
{
    public class OutputToConsoleCommand : ICommand<DeferredCommandResult>
    {
        public string Message { get; set; }
    }
}
