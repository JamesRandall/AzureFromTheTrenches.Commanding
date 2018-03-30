using System;
using System.Collections.Generic;
using System.Text;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace ServiceBusDispatchAndDequeue.Commands
{
    class SimpleCommand : ICommand
    {
        public string Message { get; set; }
    }
}
