using System;
using System.Threading.Tasks;
using AccidentalFish.Commanding;
using AccidentalFish.DependencyResolver.MicrosoftNetStandard;
using InMemoryCommanding.Actors;
using InMemoryCommanding.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace InMemoryCommanding
{
    internal class ConsoleAuditor : ICommandAuditor
    {
        public Task Audit<TCommand>(TCommand command, ICommandContext context) where TCommand : class
        {
            ConsoleColor previousColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"Type: {command.GetType()}");
            Console.WriteLine($"Correlation ID: {context.CorrelationId}");
            Console.WriteLine($"Depth: {context.Depth}");
            Console.ForegroundColor = previousColor;
            return Task.FromResult(0);
        }
    }

    internal class ConsoleAuditorFactory : ICommandAuditorFactory
    {
        public ICommandAuditor Create<TCommand>() where TCommand : class
        {
            return new ConsoleAuditor();
        }
    }

    static class ConsoleAuditing
    {
        public static async Task Run()
        {
            ICommandDispatcher dispatcher = Configure();
            ChainCommand command = new ChainCommand();
            await dispatcher.DispatchAsync(command);
            Console.WriteLine("\nPress a key to continue...");
        }

        private static ICommandDispatcher Configure()
        {
            MicrosoftNetStandardDependencyResolver resolver = new MicrosoftNetStandardDependencyResolver(new ServiceCollection());
            resolver.UseCommanding(type => resolver.Register(type, type), resetRegistry:true) // we reset the registry because we allow repeat runs, in a normal app this isn't required
                .Register<ChainCommand, ChainCommandActor>()
                .Register<OutputToConsoleCommand, OutputWorldToConsoleCommandActor>()
                .Register<OutputToConsoleCommand, OutputBigglesToConsoleCommandActor>();
            resolver.Register<ICommandAuditorFactory, ConsoleAuditorFactory>();
            resolver.BuildServiceProvider();
            return resolver.Resolve<ICommandDispatcher>();
        }
    }
}
