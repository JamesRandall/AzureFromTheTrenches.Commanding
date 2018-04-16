using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AzureEventHubAuditing;
using AzureEventHubDispatch.Commands;
using AzureFromTheTrenches.Commanding;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.AzureEventHub;
using Microsoft.Extensions.DependencyInjection;

namespace AzureEventHubDispatch
{
    class Program
    {
        private static IServiceProvider _serviceProvider;
        private static int _counter = -1;
        private const string EventHubConnectionString = "Endpoint=sb://myeventuhub.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=mykey;";
        private const string EventHubName = "demohub";

        static void Main(string[] args)
        {
            Console.WriteLine("1. Dispatch to an event hub");
            Console.WriteLine("ESC. Quit");

            do
            {
                ConsoleKeyInfo info = Console.ReadKey();

                if (info.Key == ConsoleKey.D1)
                {
#pragma warning disable 4014 // deliberate, just let it run in the background
                    RunDemo();
#pragma warning restore 4014
                }
                else if (info.Key == ConsoleKey.Escape)
                {
                    break;
                }
            } while (true);
        }

        private static async Task RunDemo()
        {
            ICommandDispatcher dispatcher = _serviceProvider == null ? Configure() : _serviceProvider.GetService<ICommandDispatcher>();
            await dispatcher.DispatchAsync(new SimpleCommand() { Message = "Hello Console" });
        }

        private static ICommandDispatcher Configure()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            ICommandingDependencyResolverAdapter dependencyResolver = serviceCollection.GetCommandingDependencyResolver(() => _serviceProvider);

            IReadOnlyDictionary<string, object> Enricher(IReadOnlyDictionary<string, object> existing) => new Dictionary<string, object> { { "ExampleEnrichedCounter", Interlocked.Increment(ref _counter) } };
            Options options = new Options
            {
                Reset = true, // we reset the registry because we allow repeat runs, in a normal app this isn't required                
                Enrichers = new[] { new FunctionWrapperCommandDispatchContextEnricher(Enricher) }
            };

            dependencyResolver.AddCommanding(options)
                .Register<SimpleCommand>(AzureEventHubDispatcherFactory.CreateCommandDispatcherFactory(EventHubConnectionString, EventHubName));            
            _serviceProvider = serviceCollection.BuildServiceProvider();
            return _serviceProvider.GetService<ICommandDispatcher>();
        }
    }
}
