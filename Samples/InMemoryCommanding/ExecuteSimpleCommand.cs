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
    static class ExecutePipelineCommand
    {
        public static async Task Run()
        {
            ICommandDispatcher dispatcher = Configure();
            await dispatcher.DispatchAsync(new PipelineCommand());
            Console.WriteLine("\nPress a key to continue...");
        }

        private static ICommandDispatcher Configure()
        {
            ServiceCollection serviceCollection = new ServiceCollection();
            Options options = new Options
            {
                Reset = true // we reset the registry because we allow repeat runs, in a normal app this isn't required                
            };

            IMicrosoftDependencyInjectionCommandingResolverAdapter dependencyResolver = serviceCollection.AddCommanding(options);

            dependencyResolver.Registry
                .Register<CancellablePipelineAwarePipelineCommandActor>(1)
                .Register<PipelineCommandActor>(2);

            dependencyResolver.ServiceProvider = serviceCollection.BuildServiceProvider();

            return dependencyResolver.ServiceProvider.GetService<ICommandDispatcher>();
        }
    }
}
