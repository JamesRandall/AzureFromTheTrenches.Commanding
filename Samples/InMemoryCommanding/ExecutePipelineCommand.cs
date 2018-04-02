using System;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.MicrosoftDependencyInjection;
using InMemoryCommanding.Commands;
using InMemoryCommanding.Handlers;
using InMemoryCommanding.Results;
using Microsoft.Extensions.DependencyInjection;

namespace InMemoryCommanding
{
    static class ExecuteSimpleCommand
    {
        public static async Task Run()
        {
            ICommandDispatcher dispatcher = Configure();
            OutputToConsoleCommand command = new OutputToConsoleCommand
            {
                Message = "Hello"
            };
            CountResult result = await dispatcher.DispatchAsync(command);
            Console.WriteLine($"{result.Count} actors called");
            await dispatcher.DispatchAsync(command);
            Console.WriteLine("\nPress a key to continue...");
        }

        private static ICommandDispatcher Configure()
        {
            ServiceCollection serviceCollection = new ServiceCollection();
            Options options = new Options
            {
                Reset = true // we reset the registry because we allow repeat runs, in a normal app this isn't required                
            };

            IMicrosoftDependencyInjectionCommandingResolverAdapter dependencyResolver = new MicrosoftDependencyInjectionCommandingResolverAdapter(serviceCollection);

            dependencyResolver.AddCommanding(options)
                .Register<OutputWorldToConsoleCommandHandler>()
                .Register<OutputBigglesToConsoleCommandHandler>();

            dependencyResolver.ServiceProvider = serviceCollection.BuildServiceProvider();

            return dependencyResolver.ServiceProvider.GetService<ICommandDispatcher>();
        }
    }
}
