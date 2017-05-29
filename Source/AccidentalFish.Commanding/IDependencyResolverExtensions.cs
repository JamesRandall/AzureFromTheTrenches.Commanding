using System;
using System.Collections.Generic;
using AccidentalFish.Commanding.Implementation;
using AccidentalFish.DependencyResolver;

namespace AccidentalFish.Commanding
{
    // ReSharper disable once InconsistentNaming
    public static class IDependencyResolverExtensions
    {
        private static ICommandRegistry _registry = null;
        private static readonly object RegistryLockObject = new object();
        private static ICommandContextEnrichment _contextEnrichment = null;
        private static readonly object EnrichmentLockObject = new object();
        private static ICommandAuditPipeline _auditorPipeline = null;
        private static readonly object AuditorPipelineLockObject = new object();
        private static ICommandDispatcherOptions _dispatcherOptions = null;
        private static readonly object DispatchOptionsLockObject = new object();

        /// <summary>
        /// Registers the commanding system in an ioc container.
        /// If the container is not able to resolve unregistered types (for example the NetStandard Microsoft container) then
        /// the commandActorContainerRegistration should be used to perform the type registration for the actor
        /// </summary>
        /// <param name="dependencyResolver">The dependency resolver to register inside</param>
        /// <param name="commandActorContainerRegistration">
        /// Unless an alternative implementation of ICommandActorFactory is supplied then actors are created through the dependency resolver
        /// but not all IoC containers can resolve unregistered concrete types (for example the built in ASP.Net Core IServiceCollection
        /// and IServiceProvider IoC cannot). Where this is the case supply an implementation for the CommandActorContainerRegistration
        /// action that registers the actors in the container. For example using an IServiceCollection instance of serviceCollection:
        ///     resolver.UseCommanding(type => services.AddTransient(type, type));
        /// </param>
        /// <returns>The dependency resolver</returns>
        public static ICommandRegistry UseCommanding(this IDependencyResolver dependencyResolver,
            Action<Type> commandActorContainerRegistration)
        {
            return UseCommanding(dependencyResolver,
                new Options {CommandActorContainerRegistration = commandActorContainerRegistration});
        }

        /// <summary>
        /// Registers the commanding system in an ioc container.
        /// If the container is not able to resolve unregistered types (for example the NetStandard Microsoft container) then
        /// the commandActorContainerRegistration should be used to perform the type registration for the actor
        /// </summary>
        /// <param name="dependencyResolver">The dependency resolver to register inside</param>
        /// <param name="options">Configuration options for the commanding system</param>
        /// <returns>The dependency resolver</returns>
        public static ICommandRegistry UseCommanding(this IDependencyResolver dependencyResolver,
            Options options = null)
        {
            options = options ?? new Options();
            // the registry is always shared, but vagaries of different IoC containers mean its dangerous to rely
            // on dependecy resolver checks for an existing registration
            lock (RegistryLockObject) 
            {
                if (_registry == null || options.Reset)
                {
                    _registry = new CommandRegistry(options.CommandActorContainerRegistration);                    
                }
                dependencyResolver.RegisterInstance(_registry);
            }

            // the enricher is always shared, but vagaries of different IoC containers mean its dangerous to rely
            // on dependecy resolver checks for an existing registration
            lock (EnrichmentLockObject) 
            {
                if (_contextEnrichment == null || options.Reset)
                {
                    _contextEnrichment = new CommandContextEnrichment(options.Enrichers ?? new List<Func<IReadOnlyDictionary<string, object>, IReadOnlyDictionary<string, object>>>());
                }
                else if (options.Enrichers != null)
                {
                    _contextEnrichment.AddEnrichers(options.Enrichers);
                }
                dependencyResolver.RegisterInstance(_contextEnrichment);
            }

            lock (AuditorPipelineLockObject)
            {
                if (_auditorPipeline == null || options.Reset)
                {
                    _auditorPipeline = new CommandAuditPipeline(t => (ICommandAuditor)dependencyResolver.Resolve(t));
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

            ICommandActorFactory commandActorFactory = new CommandActorFactory(options.CommandActorFactoryFunc ?? dependencyResolver.Resolve);
            INoResultCommandActorBaseExecuter noResultCommandActorBaseExecuter = new NoResultCommandActorBaseExecuter();
            dependencyResolver.RegisterInstance(noResultCommandActorBaseExecuter);
            dependencyResolver.RegisterInstance(commandActorFactory);
            
            dependencyResolver.Register<ICommandAuditorFactory, NullCommandAuditorFactory>();
            dependencyResolver.Register<ICommandScopeManager, AsyncLocalCommandScopeManager>();
            dependencyResolver.Register<ICommandDispatcher, CommandDispatcher>();
            dependencyResolver.Register<ICommandExecuter, CommandExecuter>();
            dependencyResolver.Register<ICommandCorrelationIdProvider, CommandCorrelationIdProvider>();
            
            return _registry;
        }

        public static IDependencyResolver RegisterCommandingAuditor<TAuditorImpl>(this IDependencyResolver dependencyResolver) where TAuditorImpl : ICommandAuditor
        {
            lock (AuditorPipelineLockObject)
            {
                if (_auditorPipeline == null)
                {
                    throw new AuditConfigurationException("The commanding system must be initialised with the UseCommanding method before any registering any auditors");
                }
                _auditorPipeline.RegisterAuditor<TAuditorImpl>();
            }
            dependencyResolver.Register<TAuditorImpl, TAuditorImpl>();
            return dependencyResolver;
        }
    }
}
