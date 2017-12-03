using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.MicrosoftDependencyInjection;
using AzureFromTheTrenches.Commanding.MicrosoftLoggingExtensions;
using InMemoryCommanding.Commands;
using InMemoryCommanding.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace InMemoryCommanding
{
    static class MicrosoftExtensionsLoggerExample
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
            serviceCollection.AddLogging(c =>
            {
                c.AddConsole(o => { o.IncludeScopes = true; });
                c.SetMinimumLevel(LogLevel.Trace);
            });

            IMicrosoftDependencyInjectionCommandingResolver dependencyResolver = new MicrosoftDependencyInjectionCommandingResolver(serviceCollection);

            Options options = new Options
            {
                Reset = true, // we reset the registry because we allow repeat runs, in a normal app this isn't required
                Enrichers = new[]
                    { new FunctionWrapperCommandDispatchContextEnricher(Enricher) }
            };
            dependencyResolver.UseCommanding(options) 
                .Register<NestingCommandHandler>()
                .Register<OutputWorldToConsoleCommandHandler>()
                .Register<OutputBigglesToConsoleCommandHandler>();
            dependencyResolver
                .UseMicrosoftLoggingExtensionsAuditor(LogLevel.Trace, LogLevel.Warning, new MicrosoftLoggingExtensionsAuditorOptions
                {
                    AuditExecuteDispatchRootOnly = auditRootOnly,
                    AuditPreDispatchRootOnly = auditRootOnly,
                    AuditPostDispatchRootOnly = auditRootOnly
                });
            dependencyResolver.ServiceProvider = serviceCollection.BuildServiceProvider();

            return dependencyResolver.ServiceProvider.GetService<ICommandDispatcher>();
        }
    }
}
