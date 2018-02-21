using System;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace CustomDispatchAndExecuter
{
    public class Handler : ICommandHandler<Command>
    {
        public async Task ExecuteAsync(Command command)
        {
            Console.WriteLine("Handler executing");
        }
    }
}
