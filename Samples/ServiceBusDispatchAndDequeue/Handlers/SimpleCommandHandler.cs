using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using ServiceBusDispatchAndDequeue.Commands;

namespace ServiceBusDispatchAndDequeue.Handlers
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
