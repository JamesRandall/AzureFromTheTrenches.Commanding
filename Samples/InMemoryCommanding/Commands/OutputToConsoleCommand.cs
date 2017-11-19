using System;
using System.Collections.Generic;
using System.Text;
using AzureFromTheTrenches.Commanding.Abstractions;
using InMemoryCommanding.Results;

namespace InMemoryCommanding.Commands
{
    public class OutputToConsoleCommand : ICommand<CountResult>
    {
        public string Message { get; set; }
    }
}
