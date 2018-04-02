using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;
using AzureFromTheTrenches.Commanding.MicrosoftDependencyInjection;
using InMemoryCommanding.Commands;
using InMemoryCommanding.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace InMemoryCommanding
{
    internal class ConsolePreDispatchAuditor : ICommandAuditor
    {
        public Task Audit(AuditItem item, CancellationToken cancellationToken)
        {
            ConsoleColor previousColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"Type: {item.CommandTypeFullName}");
            Console.WriteLine($"Correlation ID: {item.CorrelationId}");
            Console.WriteLine($"Depth: {item.Depth}");
            foreach (KeyValuePair<string, string> enrichedProperty in item.AdditionalProperties)
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

    internal class ConsolePostDispatchAuditor : ICommandAuditor
    {
        public Task Audit(AuditItem item, CancellationToken cancellationToken)
        {
            ConsoleColor previousColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"Type: {item.CommandTypeFullName}");
            Console.WriteLine($"Correlation ID: {item.CorrelationId}");
            Console.WriteLine($"Dispatch Time: {item.DispatchTimeMs}ms");
            Console.WriteLine($"Depth: {item.Depth}");
            foreach (KeyValuePair<string, string> enrichedProperty in item.AdditionalProperties)
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

    internal class ConsoleExecutionAuditor : ICommandAuditor
    {
        public Task Audit(AuditItem item, CancellationToken cancellationToken)
        {
            ConsoleColor previousColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($"Type: {item.CommandTypeFullName}");
            Console.WriteLine($"Correlation ID: {item.CorrelationId}");
            Console.WriteLine($"Execution Time: {item.ExecutionTimeMs}ms");
            Console.WriteLine($"Depth: {item.Depth}");
            foreach (KeyValuePair<string, string> enrichedProperty in item.AdditionalProperties)
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

    static class CustomConsoleAuditing
    {
        private static int _counter = -1;

        
        public static async Task Run(bool auditRootOnly)
        {
            ICommandDispatcher dispatcher = Configure(auditRootOnly);
            NestingCommand command = new NestingCommand();
            await dispatcher.DispatchAsync(command);
            Console.WriteLine("\nPress a key to continue...");
        }

        private static ICommandDispatcher Configure(bool auditRootOnly)
        {
            // we use an enricher that simply updates a counter each time enrichment occurs
            // as enrichment only occurs when the context is created this will start at 0 when the console auditing example is first run and
            // will increment by 1 on each subsequent run
            IReadOnlyDictionary<string, object> Enricher(IReadOnlyDictionary<string, object> existing) => new Dictionary<string, object> {{"Counter", Interlocked.Increment(ref _counter)}};

            ServiceCollection serviceCollection = new ServiceCollection();

            IMicrosoftDependencyInjectionCommandingResolverAdapter dependencyResolver = new MicrosoftDependencyInjectionCommandingResolverAdapter(serviceCollection);

            Options options = new Options
            {
                Reset = true, // we reset the registry because we allow repeat runs, in a normal app this isn't required
                Enrichers = new[]
                    { new FunctionWrapperCommandDispatchContextEnricher(Enricher) }
            };
            dependencyResolver.AddCommanding(options) 
                .Register<NestingCommandHandler>()
                .Register<OutputWorldToConsoleCommandHandler>()
                .Register<OutputBigglesToConsoleCommandHandler>();
            dependencyResolver
                .AddPreDispatchCommandingAuditor<ConsolePreDispatchAuditor>(auditRootOnly)
                .AddPostDispatchCommandingAuditor<ConsolePostDispatchAuditor>(auditRootOnly)
                .AddExecutionCommandingAuditor<ConsoleExecutionAuditor>(auditRootOnly);
            dependencyResolver.ServiceProvider = serviceCollection.BuildServiceProvider();
            return dependencyResolver.ServiceProvider.GetService<ICommandDispatcher>();
        }
    }
}
