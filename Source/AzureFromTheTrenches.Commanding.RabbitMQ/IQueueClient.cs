using RabbitMQ.Client;
using System.Threading.Tasks;

namespace AzureFromTheTrenches.Commanding.RabbitMQ
{
    interface IQueueClient
    {
        IConnection Connection { get; }

        IModel Channel { get; }

        void StartConsuming();

        Task SendAsync(byte[] bytes);
    }
}