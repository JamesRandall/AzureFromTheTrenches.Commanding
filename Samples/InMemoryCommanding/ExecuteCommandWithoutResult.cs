using System;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.MicrosoftDependencyInjection;
using InMemoryCommanding.Commands;
using InMemoryCommanding.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace InMemoryCommanding
{
    static class ExecuteCommandWithoutResult
    {
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

            IMicrosoftDependencyInjectionCommandingResolver dependencyResolver = new MicrosoftDependencyInjectionCommandingResolver(serviceCollection);

            dependencyResolver.UseCommanding(options)
                .Register<CommandWithoutResultHandler>()
                .Register<CancellableCommandWithoutResultHandler>();

            dependencyResolver.ServiceProvider = serviceCollection.BuildServiceProvider();

            return dependencyResolver.ServiceProvider.GetService<ICommandDispatcher>();
        }
    }
}
