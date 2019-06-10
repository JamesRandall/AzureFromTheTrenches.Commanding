using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Queue;

namespace AzureFromTheTrenches.Commanding.RabbitMQ.Implementation
{
    public class RabbitMQQueueProcessorFactory : IRabbitMQQueueProcessorFactory
    {
        private readonly ICommandExecuter _commandExecuter;
        private readonly ICommandQueueProcessorLogger _logger;
        private readonly IRabbitMQMessageSerializer _serviceBusMessageSerializer;

        public RabbitMQQueueProcessorFactory(ICommandExecuter commandExecuter,
           IRabbitMQMessageSerializer serviceBusMessageSerializer, ICommandQueueProcessorLogger logger)
        {
            _logger = logger;
            _commandExecuter = commandExecuter;
            _serviceBusMessageSerializer = serviceBusMessageSerializer;
        }

        public IRabbitMQCommandQueueProcessor Create<TCommand, TResult>(QueueClient queueClient) where TCommand : ICommand<TResult>
        {
            return new RabbitMQCommandQueueProcessor<TCommand, TResult>(
                queueClient,
                _commandExecuter,
                _serviceBusMessageSerializer,
                _logger);
        }

        public IRabbitMQCommandQueueProcessor Create<TCommand>(QueueClient queueClient) where TCommand : ICommand
        {
            return new RabbitMQCommandQueueProcessor<TCommand>(
                queueClient,
                _commandExecuter,
                _serviceBusMessageSerializer,
                _logger);
        }
    }
}
