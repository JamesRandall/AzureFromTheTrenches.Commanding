using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;
using Microsoft.Azure.EventHubs;
using Newtonsoft.Json;

namespace AzureFromTheTrenches.Commanding.AzureEventHub.Implementation
{
    internal class AzureEventHubDispatcher : ICommandDispatcher
    {
        private readonly EventHubClient _client;

        public AzureEventHubDispatcher(EventHubClient client)
        {
            _client = client;
        }

        public async Task<CommandResult<TResult>> DispatchAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = new CancellationToken())
        {
            string serializedCommand = JsonConvert.SerializeObject(command);
            await _client.SendAsync(serializedCommand);
            return new CommandResult<TResult>(default(TResult), true);
        }

        public async Task<CommandResult> DispatchAsync(ICommand command, CancellationToken cancellationToken = new CancellationToken())
        {
            string serializedCommand = JsonConvert.SerializeObject(command);
            byte[] bytes = Encoding.UTF8.GetBytes(serializedCommand);
            await _client.SendAsync(serializedCommand);
            return new CommandResult(true);
        }

        public ICommandExecuter AssociatedExecuter { get; }
    }
}
