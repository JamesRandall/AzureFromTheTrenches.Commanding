using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.RabbitMQ.Configuration;
using AzureFromTheTrenches.Commanding.RabbitMQ.Implementation;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace AzureFromTheTrenches.Commanding.RabbitMQ
{
    public class QueueClient : IQueueClient, IDisposable
    {
        public IConnection Connection => _connection;

        public IModel Channel => _channel;

        EventHandler<BasicDeliverEventArgs> _receivedCommand;
        readonly IList<ICommandHandler> _asyncCommandHandlers;
        readonly IConnection _connection;
        readonly string _queue;
        readonly IModel _channel;
        readonly EventingBasicConsumer _consumer;
        readonly object _lock = new object();
        readonly IRabbitMQMessageSerializer _serializer;

        bool _consumingStarted;
        public QueueClient(RabbitMQClientOptions options, IRabbitMQMessageSerializer serializer = null)
        {
            if (options is null)
            {
                throw new ArgumentException($"Argument {nameof(options)} is null.", nameof(options));
            }

            _queue = options.Queue;
            _serializer = serializer ?? new RabbitMQMessageSerializer();

            var factory = new ConnectionFactory
            {
                HostName = options.HostName,
                Port = options.Port,
                UserName = options.UserName,
                Password = options.Password,
                VirtualHost = options.VirtualHost,
                AutomaticRecoveryEnabled = options.AutomaticRecoveryEnabled,
                TopologyRecoveryEnabled = options.TopologyRecoveryEnabled,
                RequestedConnectionTimeout = options.RequestedConnectionTimeout,
                RequestedHeartbeat = options.RequestedHeartbeat
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: _queue, durable: options.Durable, exclusive: options.Exclusive, autoDelete: options.AutoDelete, arguments: options.QueueArguments);
            _consumer = new EventingBasicConsumer(_channel);
        }

        public void RegisterHandler(Func<byte[], CancellationToken, Task> handler)
        {
            _receivedCommand += (sender, @event) =>
            {
                handler.Invoke(@event.Body, CancellationToken.None);
                Channel.BasicAck(@event.DeliveryTag, false);
            };
        }

        public void RegisterChannelExceptionHandler(Func<object, CallbackExceptionEventArgs, Task> handler)
        {
            Channel.CallbackException += (sender, @event) =>
            {
                handler.Invoke(sender, @event);
            };
        }

        public void RegisterChannelBasicRecoverOkHandler(Func<object, EventArgs, Task> handler)
        {
            Channel.BasicRecoverOk += (sender, @event) =>
            {
                handler.Invoke(sender, @event);
            };
        }

        public void RegisterConnectionRecoveryExceptionHandler(Func<object, ConnectionRecoveryErrorEventArgs, Task> handler)
        {
            Connection.ConnectionRecoveryError += (sender, @event) =>
            {
                handler.Invoke(sender, @event);
            };
        }

        public void RegisterConnectionExceptionHandler(Func<object, CallbackExceptionEventArgs, Task> handler)
        {
            Connection.CallbackException += (sender, @event) =>
            {
                handler.Invoke(sender, @event);
            };
        }

        async Task RunAsyncHandler(ICommandHandler<ICommand> handler, ICommand message)
        {
            await handler.ExecuteAsync(message);
        }

        public void Send(byte[] body, string exchangeName = "", string routingKey = "")
        {
            var properties = CreateJsonProperties();
            routingKey = _queue;
            Send(body, properties, exchangeName, routingKey);
        }

        IBasicProperties CreateJsonProperties()
        {
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.ContentType = "application/json";
            return properties;
        }

        public void Send(byte[] bytes, IBasicProperties properties, string exchangeName, string routingKey)
        {
            lock (_lock)
            {
                _channel.BasicPublish(exchange: exchangeName,
                    routingKey: routingKey,
                    basicProperties: properties,
                    body: bytes);
            }
        }

        public async Task SendAsync(byte[] body, string exchangeName, string routingKey) =>
         await Task.Run(() => Send(body, exchangeName, routingKey)).ConfigureAwait(false);

        public async Task SendAsync(byte[] body) =>
        await Task.Run(() => Send(body)).ConfigureAwait(false);

        public void StartConsuming()
        {
            if (_consumingStarted)
            {
                return;
            }

            _consumer.Received += _receivedCommand;
            _consumingStarted = true;

            _channel.BasicConsume(queue: _queue, autoAck: false, consumer: _consumer);
        }

        public void Dispose()
        {
            if (_channel?.IsOpen == true)
            {
                _channel.Close((int)HttpStatusCode.OK, "Channel closed");
            }

            if (_connection?.IsOpen == true)
            {
                _connection.Close();
            }
        }
    }
}
