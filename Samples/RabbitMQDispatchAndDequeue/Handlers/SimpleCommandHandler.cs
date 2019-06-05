using AzureFromTheTrenches.Commanding.Abstractions;
using RabbitMQDispatchAndDequeue.Commands;
using System;
using System.Threading.Tasks;

namespace RabbitMQDispatchAndDequeue.Handlers
{
    class SimpleCommandHandler : ICommandHandler<SimpleCommand>
    {
        public Task ExecuteAsync(SimpleCommand command)
        {
            Console.WriteLine(command.Message);
            return Task.CompletedTask;
        }
    }
}
