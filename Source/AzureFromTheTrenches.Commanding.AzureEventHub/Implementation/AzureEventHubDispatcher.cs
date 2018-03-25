using System;
using System.Threading;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;
using Newtonsoft.Json;

namespace AzureFromTheTrenches.Commanding.AzureEventHub.Implementation
{
    internal class AzureEventHubDispatcher : ICommandDispatcher
    {
        private readonly IEventHubClient _client;
        private readonly Func<ICommand, string> _getPartitionKeyFunc;

        public AzureEventHubDispatcher(IEventHubClient client, Func<ICommand, string> getPartitionKeyFunc=null)
        {
            _client = client;
            _getPartitionKeyFunc = getPartitionKeyFunc;
        }

        public async Task<CommandResult<TResult>> DispatchAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = new CancellationToken())
        {
            string serializedCommand = JsonConvert.SerializeObject(command);
            if (_getPartitionKeyFunc != null)
            {
                await _client.SendAsync(serializedCommand, _getPartitionKeyFunc(command));
            }
            else
            {
                await _client.SendAsync(serializedCommand);
            }
            
            return new CommandResult<TResult>(default(TResult), true);
        }

        public async Task<CommandResult> DispatchAsync(ICommand command, CancellationToken cancellationToken = new CancellationToken())
        {
            string serializedCommand = JsonConvert.SerializeObject(command);
            if (_getPartitionKeyFunc != null)
            {
                await _client.SendAsync(serializedCommand, _getPartitionKeyFunc(command));
            }
            else
            {
                await _client.SendAsync(serializedCommand);
            }
            return new CommandResult(true);
        }

        public ICommandExecuter AssociatedExecuter { get; }
    }
}
