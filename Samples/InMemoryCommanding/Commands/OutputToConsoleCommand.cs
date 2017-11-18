using System;
using System.Collections.Generic;
using System.Text;
using AccidentalFish.Commanding.Abstractions;
using AccidentalFish.Commanding.Abstractions.Model;
using InMemoryCommanding.Results;

namespace InMemoryCommanding.Commands
{
    public class OutputToConsoleCommand : ICommand<CountResult>
    {
        public string Message { get; set; }
    }
}
