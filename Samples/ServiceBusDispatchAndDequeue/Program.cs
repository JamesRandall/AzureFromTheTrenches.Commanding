using System;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.AzureServiceBus;
using AzureFromTheTrenches.Commanding.Queue;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using ServiceBusDispatchAndDequeue.Commands;
using ServiceBusDispatchAndDequeue.Handlers;

namespace ServiceBusDispatchAndDequeue
{
    class Program
    {
        private const string ServiceBusConnectionString = "Endpoint=sb://serverlesswebcrawler.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=yourkey";
        private static int _counter = 0;
        private static IServiceBusCommandQueueProcessor _commandQueueProcessor;
        private static IServiceProvider _dispatchServiceProvider;
        private static IServiceProvider _dequeueServiceProvider;

        static void Main(string[] args)
        {
            Console.WriteLine("1. Queue command");
            Console.WriteLine("2. Launch queue processor");
            Console.WriteLine("Esc - quit");

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

                    case ConsoleKey.D2:
#pragma warning disable 4014
                        LaunchQueueProcessor();
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
                Message = $"Hello World {_counter++}"
            });
            Console.WriteLine("Command dispatched");
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
                resolver.AddAzureServiceBus();

                // register our command to dispatch to a servie bus queue
                QueueClient client = new QueueClient(ServiceBusConnectionString, "myqueue");
                commandRegistry.Register<SimpleCommand>(client.CreateCommandDispatcherFactory());

                _dispatchServiceProvider = serviceCollection.BuildServiceProvider();
            }
            
            
            ICommandDispatcher dispatcher = _dispatchServiceProvider.GetService<ICommandDispatcher>();
            return dispatcher;
        }

        static void LaunchQueueProcessor()
        {
            IServiceBusCommandQueueProcessorFactory factory = ConfigureForDequeue();
            QueueClient client = new QueueClient(ServiceBusConnectionString, "myqueue");

            _commandQueueProcessor = factory.Create<SimpleCommand>(client);
        }

        private static IServiceBusCommandQueueProcessorFactory ConfigureForDequeue()
        {
            if (_dequeueServiceProvider == null)
            {
                IServiceCollection serviceCollection = new ServiceCollection();
                CommandingDependencyResolverAdapter resolver = new CommandingDependencyResolverAdapter(
                    (fromType, toInstance) => serviceCollection.AddSingleton(fromType, toInstance),
                    (fromType, toType) => serviceCollection.AddTransient(fromType, toType),
                    (resolveType) => _dispatchServiceProvider.GetService(resolveType));
                ICommandRegistry commandRegistry = resolver.AddCommanding();
                resolver.AddQueues().AddAzureServiceBus();

                // register our command to dispatch to a servie bus queue
                commandRegistry.Register<SimpleCommandHandler>();

                _dequeueServiceProvider = serviceCollection.BuildServiceProvider();
            }
            
            IServiceBusCommandQueueProcessorFactory serviceBusCommandQueueProcessorFactory = _dispatchServiceProvider.GetService<IServiceBusCommandQueueProcessorFactory>();
            return serviceBusCommandQueueProcessorFactory;
        }
    }
}
