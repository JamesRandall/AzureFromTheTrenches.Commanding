using System;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding;
using AzureFromTheTrenches.Commanding.Abstractions;
using InMemoryCommanding.Commands;
using InMemoryCommanding.Handlers;
using InMemoryCommanding.Results;
using Microsoft.Extensions.DependencyInjection;

namespace InMemoryCommanding
{
    static class ExecuteCommandWithoutResult
    {
        private static IServiceProvider _serviceProvider;

        public static async Task Run()
        {
            ICommandDispatcher dispatcher = Configure();
            CommandWithoutResult command = new CommandWithoutResult
            {
                DoSomething = "Hello"
            };
            await dispatcher.DispatchAsync(command);
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

            CommandingDependencyResolver dependencyResolver = serviceCollection.GetCommandingDependencyResolver(() => _serviceProvider);

            dependencyResolver.UseCommanding(options)
                .Register<CommandWithoutResultHandler>();

            _serviceProvider = serviceCollection.BuildServiceProvider();

            return _serviceProvider.GetService<ICommandDispatcher>();
        }
    }
}
