using System;
using System.Threading.Tasks;
using AccidentalFish.Commanding;
using AccidentalFish.Commanding.Abstractions;
using AccidentalFish.DependencyResolver.MicrosoftNetStandard;
using InMemoryCommanding.Actors;
using InMemoryCommanding.Commands;
using InMemoryCommanding.Results;
using Microsoft.Extensions.DependencyInjection;

namespace InMemoryCommanding
{
    static class ExecuteSimpleCommand
    {
        private static IServiceProvider _serviceProvider;

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
                CommandActorContainerRegistration = type => serviceCollection.AddTransient(type, type),
                Reset = true // we reset the registry because we allow repeat runs, in a normal app this isn't required                
            };

            CommandingDependencyResolver dependencyResolver = serviceCollection.GetCommandingDependencyResolver(() => _serviceProvider);

            IDependencyResolverExtensions.UseCommanding(dependencyResolver, options)
                .Register<OutputToConsoleCommand, CountResult, OutputWorldToConsoleCommandActor>()
                .Register<OutputToConsoleCommand, CountResult, OutputBigglesToConsoleCommandActor>();

            _serviceProvider = serviceCollection.BuildServiceProvider();

            return _serviceProvider.GetService<ICommandDispatcher>();
        }
    }
}
