using AzureFromTheTrenches.Commanding;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Queue;
using AzureFromTheTrenches.Commanding.RabbitMQ;
using AzureFromTheTrenches.Commanding.RabbitMQ.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQDispatchAndDequeue.Commands;
using RabbitMQDispatchAndDequeue.Handlers;
using System;
using System.Threading.Tasks;

namespace RabbitMQDispatchAndDequeue
{
    class Program
    {
        public static IConfiguration Configuration { get; set; }
        private static IServiceProvider _dispatchServiceProvider;
        private static IServiceProvider _dequeueServiceProvider;

        static void Main(string[] args)
        {
            Console.WriteLine("1. Queue command");
            Console.WriteLine("Esc - quit");
            LaunchQueueProcessor();

            ConsoleKeyInfo keyInfo;
            do
            {
                keyInfo = Console.ReadKey();

                switch (keyInfo.Key)
                {
                    case ConsoleKey.D1:
#pragma warning disable 4014
                        RunDispatchDemo();
#pragma warning restore 4014
                        break;
                }
            } while (keyInfo.Key != ConsoleKey.Escape);
        }

        static async Task RunDispatchDemo()
        {
            // Normally the dispatcher would be injected into, say, a ASP.Net controller
            ICommandDispatcher dispatcher = ConfigureForDispatchToQueue();

            // Dispatch a command
            await dispatcher.DispatchAsync(new SimpleCommand
            {
                Message = $"Hello World"
            });
        }

        private static ICommandDispatcher ConfigureForDispatchToQueue()
        {
            if (_dispatchServiceProvider == null)
            {
                IServiceCollection serviceCollection = new ServiceCollection();
                CommandingDependencyResolverAdapter resolver = new CommandingDependencyResolverAdapter(
                    (fromType, toInstance) => serviceCollection.AddSingleton(fromType, toInstance),
                    (fromType, toType) => serviceCollection.AddTransient(fromType, toType),
                    (resolveType) => _dispatchServiceProvider.GetService(resolveType));
                ICommandRegistry commandRegistry = resolver.AddCommanding();
                resolver.AddRabbitMq();
                _dispatchServiceProvider = serviceCollection.BuildServiceProvider();

                var client = new QueueClient(new RabbitMQClientOptions() { Queue = "HelloTrenches" });

                // register our command to dispatch to a RabbitMq queue
                commandRegistry.Register<SimpleCommand>(client.CreateCommandDispatcherFactory());
            }
            ICommandDispatcher dispatcher = _dispatchServiceProvider.GetService<ICommandDispatcher>();
            return dispatcher;
        }

        static void LaunchQueueProcessor()
        {
            var queue = new QueueClient(new RabbitMQClientOptions() { Queue = "HelloTrenches" });

            IRabbitMQQueueProcessorFactory factory = ConfigureForDequeue();
            factory.Create<SimpleCommand>(queue);
        }

        private static IRabbitMQQueueProcessorFactory ConfigureForDequeue()
        {
            if (_dequeueServiceProvider == null)
            {
                IServiceCollection serviceCollection = new ServiceCollection();
                CommandingDependencyResolverAdapter resolver = new CommandingDependencyResolverAdapter(
                    (fromType, toInstance) => serviceCollection.AddSingleton(fromType, toInstance),
                    (fromType, toType) => serviceCollection.AddTransient(fromType, toType),
                    (resolveType) => _dequeueServiceProvider.GetService(resolveType));

                resolver.AddQueues(logError: ConsoleLogger, logInfo: ConsoleLogger, logWarning: ConsoleLogger).AddRabbitMq();
                ICommandRegistry commandRegistry = resolver.AddCommanding();

                commandRegistry.Register<SimpleCommandHandler>();
                _dequeueServiceProvider = serviceCollection.BuildServiceProvider();
            }

            IRabbitMQQueueProcessorFactory serviceBusCommandQueueProcessorFactory = _dequeueServiceProvider.GetService<IRabbitMQQueueProcessorFactory>();
            return serviceBusCommandQueueProcessorFactory;
        }

        static void ConsoleLogger(string message, ICommand command, Exception ex)
        {
            Console.WriteLine($"Message: {message}" + command ?? $"Command: {command}" + ex ?? $"Ex: {ex}");
        }
    }
}
