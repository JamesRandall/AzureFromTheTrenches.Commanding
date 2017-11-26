using System;
using System.Collections.Generic;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Implementation;

namespace AzureFromTheTrenches.Commanding
{
    public static class CommandingDependencies
    {
        private static ICommandRegistry _registry = null;
        private static readonly object RegistryLockObject = new object();
        private static ICommandDispatchContextEnrichment _dispatchContextEnrichment = null;
        private static readonly object EnrichmentLockObject = new object();
        private static ICommandAuditPipeline _auditorPipeline = null;
        private static readonly object AuditorPipelineLockObject = new object();
        private static ICommandDispatcherOptions _dispatcherOptions = null;
        private static readonly object DispatchOptionsLockObject = new object();

        /// <summary>
        /// Registers the commanding system in an ioc container.
        /// If the container is not able to resolve unregistered types (for example the NetStandard Microsoft container) then
        /// the commandActorContainerRegistration should be used to perform the type registration for the handler
        /// </summary>
        /// <param name="dependencyResolver">The dependency resolver to register inside</param>
        /// <param name="commandActorContainerRegistration">
        /// Unless an alternative implementation of ICommandHandlerFactory is supplied then actors are created through the dependency resolver
        /// but not all IoC containers can resolve unregistered concrete types (for example the built in ASP.Net Core IServiceCollection
        /// and IServiceProvider IoC cannot). Where this is the case supply an implementation for the CommandActorContainerRegistration
        /// action that registers the actors in the container. For example using an IServiceCollection instance of serviceCollection:
        ///     resolver.UseCommanding(type => services.AddTransient(type, type));
        /// </param>
        /// <returns>The dependency resolver</returns>
        public static ICommandRegistry UseCommanding(this ICommandingDependencyResolver dependencyResolver,
            Action<Type> commandActorContainerRegistration)
        {
            return UseCommanding(dependencyResolver,
                new Options {CommandActorContainerRegistration = commandActorContainerRegistration});
        }

        /// <summary>
        /// Registers the commanding system in an ioc container.
        /// If the container is not able to resolve unregistered types (for example the NetStandard Microsoft container) then
        /// the commandActorContainerRegistration should be used to perform the type registration for the handler
        /// </summary>
        /// <param name="dependencyResolver">The dependency resolver to register inside</param>
        /// <param name="options">Configuration options for the commanding system</param>
        /// <returns>The dependency resolver</returns>
        public static ICommandRegistry UseCommanding(this ICommandingDependencyResolver dependencyResolver,
            Options options = null)
        {
            options = options ?? new Options();
            // the registry is always shared, but vagaries of different IoC containers mean its dangerous to rely
            // on dependecy resolver checks for an existing registration
            lock (RegistryLockObject)
            {
                if (_registry == null || options.Reset)
                {
                    Action<Type> resolverContainerRegistration = type => dependencyResolver.TypeMapping(type, type);
                    _registry = new CommandRegistry(options.CommandActorContainerRegistration ?? resolverContainerRegistration);
                }
                dependencyResolver.RegisterInstance(_registry);
            }

            // the enricher is always shared, but vagaries of different IoC containers mean its dangerous to rely
            // on dependecy resolver checks for an existing registration
            lock (EnrichmentLockObject)
            {
                if (_dispatchContextEnrichment == null || options.Reset)
                {
                    _dispatchContextEnrichment = new CommandDispatchContextEnrichment(options.Enrichers ?? new List<ICommandDispatchContextEnricher>());
                }
                else if (options.Enrichers != null)
                {
                    _dispatchContextEnrichment.AddEnrichers(options.Enrichers);
                }
                dependencyResolver.RegisterInstance(_dispatchContextEnrichment);
            }

            lock (AuditorPipelineLockObject)
            {
                if (_auditorPipeline == null || options.Reset)
                {
                    _auditorPipeline = new CommandAuditPipeline(t => (ICommandAuditor)dependencyResolver.Resolve(t), dependencyResolver.Resolve<ICommandAuditSerializer>);
                }
                dependencyResolver.RegisterInstance(_auditorPipeline);
            }

            lock (DispatchOptionsLockObject)
            {
                if (_dispatcherOptions == null || options.Reset)
                {
                    _dispatcherOptions = new CommandDispatcherOptions(options.AuditRootCommandOnly);
                }
                else
                {
                    if (options.AuditRootCommandOnly.HasValue)
                    {
                        if (_dispatcherOptions.AuditRootCommandOnly.HasValue)
                        {
                            throw new AuditConfigurationException("The AuditRootCommandOnly option has already been specified");
                        }
                        _dispatcherOptions = new CommandDispatcherOptions(options.AuditRootCommandOnly);
                    }
                }
                dependencyResolver.RegisterInstance(_dispatcherOptions);
            }

            ICommandHandlerFactory commandHandlerFactory = new CommandHandlerFactory(options.CommandActorFactoryFunc ?? dependencyResolver.Resolve);
            ICommandActorExecuter commandActorExecuter = new CommandActorExecuter();
            ICommandActorChainExecuter commandActorChainExecuter = new CommandActorChainExecuter();
            dependencyResolver.RegisterInstance(commandHandlerFactory);
            dependencyResolver.RegisterInstance(commandActorExecuter);
            dependencyResolver.RegisterInstance(commandActorChainExecuter);

            dependencyResolver.TypeMapping<ICommandAuditorFactory, NullCommandAuditorFactory>();
            dependencyResolver.TypeMapping<ICommandScopeManager, AsyncLocalCommandScopeManager>();
            dependencyResolver.TypeMapping<ICommandDispatcher, CommandDispatcher>();
            dependencyResolver.TypeMapping<ICommandExecuter, CommandExecuter>();
            dependencyResolver.TypeMapping<ICommandCorrelationIdProvider, CommandCorrelationIdProvider>();
            dependencyResolver.TypeMapping<ICommandAuditSerializer, CommandAuditSerializer>();

            return _registry;
        }

        public static ICommandingDependencyResolver UseCommandingAuditor<TAuditorImpl>(this ICommandingDependencyResolver dependencyResolver) where TAuditorImpl : ICommandAuditor
        {
            lock (AuditorPipelineLockObject)
            {
                if (_auditorPipeline == null)
                {
                    throw new AuditConfigurationException("The commanding system must be initialised with the UseCommanding method before any registering any auditors");
                }
                IAuditorRegistration registration = (IAuditorRegistration)_auditorPipeline;
                registration.RegisterAuditor<TAuditorImpl>();
            }
            dependencyResolver.TypeMapping<TAuditorImpl, TAuditorImpl>();
            return dependencyResolver;
        }
    }
}
