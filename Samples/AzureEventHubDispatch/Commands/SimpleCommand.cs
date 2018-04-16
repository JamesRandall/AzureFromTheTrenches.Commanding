using System;
using System.Collections.Generic;
using System.Text;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureEventHubDispatch.Commands
{
    class SimpleCommand : ICommand
    {
        public string Message { get; set; }
    }
}
