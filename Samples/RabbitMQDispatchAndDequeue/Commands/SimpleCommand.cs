using AzureFromTheTrenches.Commanding.Abstractions;

namespace RabbitMQDispatchAndDequeue.Commands
{
    class SimpleCommand : ICommand
    {
        public string Message { get; set; }
    }
}
