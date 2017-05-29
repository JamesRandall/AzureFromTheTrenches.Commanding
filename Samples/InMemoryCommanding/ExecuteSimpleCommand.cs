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
            Options options = new Options
            {
                CommandActorContainerRegistration = type => resolver.Register(type, type),
                Reset = true // we reset the registry because we allow repeat runs, in a normal app this isn't required                
            };
            resolver.UseCommanding(options)
                .Register<OutputToConsoleCommand, OutputWorldToConsoleCommandActor>()
                .Register<OutputToConsoleCommand, OutputBigglesToConsoleCommandActor>();
            resolver.BuildServiceProvider();
            return resolver.Resolve<ICommandDispatcher>();
        }
    }
}
