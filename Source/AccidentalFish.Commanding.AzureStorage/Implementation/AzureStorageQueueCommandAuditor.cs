using System;
using System.Linq;
using System.Threading.Tasks;
using AccidentalFish.Commanding.AzureStorage.Model;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace AccidentalFish.Commanding.AzureStorage.Implementation
{
    internal class AzureStorageQueueCommandAuditor : ICommandAuditor
    {
        private readonly ICloudQueueProvider _cloudQueueProvider;

        public AzureStorageQueueCommandAuditor(ICloudQueueProvider cloudQueueProvider)
        {
            _cloudQueueProvider = cloudQueueProvider;
        }

        public async Task Audit<TCommand>(TCommand command, ICommandDispatchContext dispatchContext) where TCommand : class
        {
            CloudQueue queue = _cloudQueueProvider.Queue;
            DateTime recordedAt = DateTime.UtcNow;
            Guid commandId = Guid.NewGuid();
            string commandType = command.GetType().AssemblyQualifiedName;

            AuditQueueItem item = new AuditQueueItem
            {
                AdditionalProperties = dispatchContext.AdditionalProperties.ToDictionary(x => x.Key, x => x.Value.ToString()),
                CommandType = commandType,
                CorrelationId = dispatchContext.CorrelationId,
                Depth = dispatchContext.Depth,
                CommandId = commandId,
                RecordedAtUtc = recordedAt
            };
            string json = JsonConvert.SerializeObject(item);
            await queue.AddMessageAsync(new CloudQueueMessage(json));
        }
    }
}
