using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace AzureFromTheTrenches.Commanding.MicrosoftDependencyInjection
{
    // ReSharper disable once InconsistentNaming
    [Obsolete("Please use MicrosoftDependencyInjectionCommandingResolverAdapter instead")]
    public class MicrosoftDependencyInjectionCommandingResolver : IMicrosoftDependencyInjectionCommandingResolver
    {
        private readonly IServiceCollection _serviceCollection;

        public MicrosoftDependencyInjectionCommandingResolver(IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;            
        }

        public ICommandingDependencyResolver RegisterInstance<TType>(TType instance)
        {
            _serviceCollection.AddSingleton(typeof(TType), instance);
            return this;
        }

        public ICommandingDependencyResolver TypeMapping<TType, TImpl>() where TImpl : TType
        {
            _serviceCollection.AddTransient(typeof(TType), typeof(TImpl));
            return this;
        }

        public ICommandingDependencyResolver TypeMapping(Type type, Type impl)
        {
            _serviceCollection.AddTransient(type, impl);
            return this;
        }

        public TType Resolve<TType>()
        {
            return GetServiceProvider().GetService<TType>();
        }

        public object Resolve(Type type)
        {
            return GetServiceProvider().GetService(type);
        }

        public ICommandingRuntime AssociatedCommandingRuntime { get; set; }

        public ICommandRegistry Registry { get; internal set; }

        public IServiceProvider ServiceProvider { get; set; }

        private IServiceProvider GetServiceProvider()
        {
            if (ServiceProvider == null)
            {
                throw new CommandingDependencyInjectorException("An instance of IServiceProvider must be set using the ServiceProvider property before executing commands.");
            }
            return ServiceProvider;
        }
    }
}
