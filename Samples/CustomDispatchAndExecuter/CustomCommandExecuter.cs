using System;
using System.Threading;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace CustomDispatchAndExecuter
{
    public class CustomCommandExecuter : ICommandExecuter
    {
        public Task<TResult> ExecuteAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = new CancellationToken())
        {
            Console.WriteLine("CustomCommandExecuter executing");
            return Task.FromResult(default(TResult));
        }
    }
}
