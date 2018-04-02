using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Tests.Acceptance.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace AzureFromTheTrenches.Commanding.Tests.Acceptance
{
    public abstract class AbstractDispatchTestBase
    {
        protected AbstractDispatchTestBase(Action<ICommandRegistry, CustomDispatcher> registrations)
        {
            var serviceCollection = new ServiceCollection();
            var resolver = new CommandingDependencyResolverAdapter(
                (type, instance) => serviceCollection.AddSingleton(type, instance),
                (type, impl) => serviceCollection.AddTransient(type, impl),
                type => ServiceProvider.GetService(type)
            );
            CommandingConfiguration = new CommandingRuntime();
            var registry = CommandingConfiguration.AddCommanding(resolver);
            CommandTracer = new CommandTracer();
            serviceCollection.AddSingleton(CommandTracer);
            CustomDispatcher = new CustomDispatcher();
            CustomExecuter = new CustomExecuter();

            registrations(registry, CustomDispatcher);
            ServiceProvider = serviceCollection.BuildServiceProvider();
            Dispatcher = ServiceProvider.GetRequiredService<ICommandDispatcher>();
            
        }

        protected CommandingRuntime CommandingConfiguration { get; }

        protected IServiceProvider ServiceProvider { get; }

        protected ICommandDispatcher Dispatcher { get; }

        protected ICommandTracer CommandTracer { get; }

        protected CustomDispatcher CustomDispatcher { get; }

        protected CustomExecuter CustomExecuter { get; }
    }
}
