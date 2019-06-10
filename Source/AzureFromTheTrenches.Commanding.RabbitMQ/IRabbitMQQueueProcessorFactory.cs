using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.RabbitMQ
{
    public interface IRabbitMQQueueProcessorFactory
    {
        IRabbitMQCommandQueueProcessor Create<TCommand, TResult>(
            QueueClient queueClient) where TCommand : ICommand<TResult>;

        IRabbitMQCommandQueueProcessor Create<TCommand>(
            QueueClient queueClient) where TCommand : ICommand;
    }
}
