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
    }
}
