using AccidentalFish.Commanding.Implementation;
using AccidentalFish.DependencyResolver;

namespace AccidentalFish.Commanding
{
    // ReSharper disable once InconsistentNaming
    public static class IDependencyResolverExtensions
    {
        public static ICommandRegistry UseCommanding(this IDependencyResolver dependencyResolver)
        {
            ICommandRegistry registry = null;
            if (!dependencyResolver.IsRegistered<ICommandRegistry>())
            {
                registry = new CommandRegistry();
                dependencyResolver.RegisterInstance(registry);
            }
            else
            {
                registry = dependencyResolver.Resolve<ICommandRegistry>();
            }
            dependencyResolver.Register<ICommandDispatcher, CommandDispatcher>();
            dependencyResolver.Register<ICommandExecuter, CommandExecuter>();
            
            return registry;
        }
    }
}
