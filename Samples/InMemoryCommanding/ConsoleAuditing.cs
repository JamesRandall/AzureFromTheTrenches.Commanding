using System;
using System.Collections.Generic;
using System.Threading;
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
        public Task AuditWithCommandPayload<TCommand>(TCommand command, Guid commandId, ICommandDispatchContext dispatchContext) where TCommand : class
        {
            ConsoleColor previousColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"Type: {command.GetType()}");
            Console.WriteLine($"Correlation ID: {dispatchContext.CorrelationId}");
            Console.WriteLine($"Depth: {dispatchContext.Depth}");
            foreach (KeyValuePair<string, object> enrichedProperty in dispatchContext.AdditionalProperties)
            {
                Console.WriteLine($"{enrichedProperty.Key}: {enrichedProperty.Value}");
            }
            Console.ForegroundColor = previousColor;
            return Task.FromResult(0);
        }

        public Task AuditWithNoPayload(Guid commandId, string commandType, ICommandDispatchContext dispatchContext)
        {
            throw new NotImplementedException();
        }
    }

    static class ConsoleAuditing
    {
        private static int _counter = -1;

        public static async Task Run(bool auditRootOnly)
        {
            ICommandDispatcher dispatcher = Configure(auditRootOnly);
            ChainCommand command = new ChainCommand();
            await dispatcher.DispatchAsync(command);
            Console.WriteLine("\nPress a key to continue...");
        }

        private static ICommandDispatcher Configure(bool auditRootOnly)
        {
            // we use an enricher that simply updates a counter each time enrichment occurs
            // as enrichment only occurs when the context is created this will start at 0 when the console auditing example is first run and
            // will increment by 1 on each subsequent run
            IReadOnlyDictionary<string, object> Enricher(IReadOnlyDictionary<string, object> existing) => new Dictionary<string, object> {{"Counter", Interlocked.Increment(ref _counter)}};

            MicrosoftNetStandardDependencyResolver resolver = new MicrosoftNetStandardDependencyResolver(new ServiceCollection());
            Options options = new Options
            {
                CommandActorContainerRegistration = type => resolver.Register(type, type),
                Reset = true, // we reset the registry because we allow repeat runs, in a normal app this isn't required
                Enrichers = new[]
                    { new FunctionWrapperCommandDispatchContextEnricher(Enricher) },
                AuditRootCommandOnly = auditRootOnly
            };
            resolver.UseCommanding(options) 
                .Register<ChainCommand, ChainCommandActor>()
                .Register<OutputToConsoleCommand, OutputWorldToConsoleCommandActor>()
                .Register<OutputToConsoleCommand, OutputBigglesToConsoleCommandActor>();
            resolver.RegisterCommandingAuditor<ConsoleAuditor>();
            resolver.BuildServiceProvider();
            return resolver.Resolve<ICommandDispatcher>();
        }
    }
}
