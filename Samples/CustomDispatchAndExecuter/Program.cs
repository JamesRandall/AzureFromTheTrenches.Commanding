using System;
using AzureFromTheTrenches.Commanding;
using AzureFromTheTrenches.Commanding.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace CustomDispatchAndExecuter
{
    class Program
    {
        static void Main(string[] args)
        {
            IServiceProvider serviceProvider = null;

            var serviceCollection = new ServiceCollection()
                .AddTransient<CustomCommandExecuter>()
                .AddTransient<CustomCommandDispatcher>();

            ICommandRegistry registry = new CommandingDependencyResolver(
                    (type, instance) => serviceCollection.AddSingleton(type, instance),
                    (type, impl) => serviceCollection.AddTransient(type, impl),
                    type => serviceProvider.GetService(type)
                )
                .UseCommanding();

            registry.Register<Command>(() => serviceProvider.GetService<CustomCommandDispatcher>());

            serviceProvider = serviceCollection.BuildServiceProvider();

            serviceProvider.GetService<ICommandDispatcher>().DispatchAsync(new Command());

            Console.ReadKey();
        }
    }
}
