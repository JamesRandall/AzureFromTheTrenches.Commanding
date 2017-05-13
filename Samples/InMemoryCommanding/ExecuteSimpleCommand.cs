using System;
using System.Threading.Tasks;
using AccidentalFish.Commanding;
using AccidentalFish.DependencyResolver.MicrosoftNetStandard;
using InMemoryCommanding.Actors;
using InMemoryCommanding.Commands;
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
            CountResult result = await dispatcher.DispatchAsync<OutputToConsoleCommand, CountResult>(command);
            Console.WriteLine($"{result.Count} actors called");
            await dispatcher.DispatchAsync(command);
            Console.WriteLine("\nPress a key to continue...");
        }

        private static ICommandDispatcher Configure()
        {
            MicrosoftNetStandardDependencyResolver resolver = new MicrosoftNetStandardDependencyResolver(new ServiceCollection());
            resolver.UseCommanding(type => resolver.Register(type, type))
                .Register<OutputToConsoleCommand, OutputWorldToConsoleCommandActor>()
                .Register<OutputToConsoleCommand, OutputBigglesToConsoleCommandActor>();
            resolver.BuildServiceProvider();
            return resolver.Resolve<ICommandDispatcher>();
        }
    }
}
