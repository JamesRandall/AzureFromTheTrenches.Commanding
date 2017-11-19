using System;
using System.Threading.Tasks;
using AccidentalFish.Commanding;
using AccidentalFish.Commanding.Abstractions;
using InMemoryCommanding.Actors;
using InMemoryCommanding.Commands;
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
                CommandActorContainerRegistration = type => serviceCollection.AddTransient(type, type),
                Reset = true // we reset the registry because we allow repeat runs, in a normal app this isn't required                
            };

            CommandingDependencyResolver dependencyResolver = serviceCollection.GetCommandingDependencyResolver(() => _serviceProvider);

            dependencyResolver.UseCommanding(options)
                .Register<CommandWithoutResult, CommandWithoutResultActor>();

            _serviceProvider = serviceCollection.BuildServiceProvider();

            return _serviceProvider.GetService<ICommandDispatcher>();
        }
    }
}
