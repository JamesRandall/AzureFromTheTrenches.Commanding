using System;
using AccidentalFish.Commanding.Implementation;
using AccidentalFish.DependencyResolver;

namespace AccidentalFish.Commanding
{
    // ReSharper disable once InconsistentNaming
    public static class IDependencyResolverExtensions
    {
        private static ICommandRegistry _registry = null;
        private static readonly object RegistryLockObject = new object();

        /// <summary>
        /// Registers the commanding system in an ioc container.
        /// If the container is not able to resolve unregistered types (for example the NetStandard Microsoft container) then
        /// the commandActorContainerRegistration should be used to perform the type registration for the actor
        /// </summary>
        /// <param name="dependencyResolver">The dependency resolver to register in</param>
        /// <param name="commandActorContainerRegistration">Optional ioc container action</param>
        /// <param name="commandActorFactoryFunc">An optional function that if supplied will be used to create actors based on their type, if null then the dependency resolver will be used</param>
        /// <returns></returns>
        public static ICommandRegistry UseCommanding(this IDependencyResolver dependencyResolver,
            Action<Type> commandActorContainerRegistration = null,
            Func<Type, object> commandActorFactoryFunc = null)
        {
            lock (RegistryLockObject) // the registry is always shared, but vagaries of different IoC containers mean its not good enough to rely on dependecy resolver checks
            {
                if (_registry == null)
                {
                    _registry = new CommandRegistry(commandActorContainerRegistration);                    
                }
                dependencyResolver.RegisterInstance(_registry);
            }
            
            ICommandActorFactory commandActorFactory = new CommandActorFactory(commandActorFactoryFunc ?? dependencyResolver.Resolve);
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
