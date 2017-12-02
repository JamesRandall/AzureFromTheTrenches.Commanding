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
    static class ExecutePipelineCommand
    {
        private static IServiceProvider _serviceProvider;

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

            CommandingDependencyResolver dependencyResolver = serviceCollection.GetCommandingDependencyResolver(() => _serviceProvider);

            dependencyResolver.UseCommanding(options)
                .Register<CancellablePipelineAwarePipelineCommandActor>(1)
                .Register<PipelineCommandActor>(2);

            _serviceProvider = serviceCollection.BuildServiceProvider();

            return _serviceProvider.GetService<ICommandDispatcher>();
        }
    }
}
