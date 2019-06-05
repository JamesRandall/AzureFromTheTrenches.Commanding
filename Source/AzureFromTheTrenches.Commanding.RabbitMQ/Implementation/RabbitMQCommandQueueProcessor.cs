using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;
using AzureFromTheTrenches.Commanding.Queue;
using System.Threading;
using System.Threading.Tasks;

namespace AzureFromTheTrenches.Commanding.RabbitMQ.Implementation
{
    internal class RabbitMQCommandQueueProcessor<TCommand, TResult> : AbstractRabbitMQCommandQueueProcessor<TCommand> where TCommand : ICommand<TResult>
    {
        public RabbitMQCommandQueueProcessor(QueueClient queueClient,
          ICommandExecuter commandExecuter,
          IRabbitMQMessageSerializer serializer,
          ICommandQueueProcessorLogger logger) : base(queueClient, logger, commandExecuter, serializer)
        {
        }

        protected override Task ExecuteCommandAsync(TCommand command, CancellationToken cancellationToken)
        {
            ICommand<TResult> castCommand = (ICommand<TResult>)command;
            return CommandExecuter.ExecuteAsync(castCommand, cancellationToken);
        }
    }

    internal class RabbitMQCommandQueueProcessor<TCommand> : AbstractRabbitMQCommandQueueProcessor<TCommand> where TCommand : ICommand
    {
        public RabbitMQCommandQueueProcessor(QueueClient queueClient,
            ICommandExecuter commandExecuter,
            IRabbitMQMessageSerializer serializer,
            ICommandQueueProcessorLogger logger) : base(queueClient, logger, commandExecuter, serializer)
        {
        }

        protected override Task ExecuteCommandAsync(TCommand command, CancellationToken cancellationToken)
        {
            NoResultCommandWrapper wrapper = new NoResultCommandWrapper(command);
            return CommandExecuter.ExecuteAsync(wrapper, cancellationToken);
        }
    }
}
