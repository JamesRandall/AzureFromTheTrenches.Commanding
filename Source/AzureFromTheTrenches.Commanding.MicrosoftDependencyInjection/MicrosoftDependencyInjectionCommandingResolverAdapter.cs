using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace AzureFromTheTrenches.Commanding.MicrosoftDependencyInjection
{
    public class MicrosoftDependencyInjectionCommandingResolverAdapter : IMicrosoftDependencyInjectionCommandingResolverAdapter
    {
        private readonly IServiceCollection _serviceCollection;

        public MicrosoftDependencyInjectionCommandingResolverAdapter(IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection;
        }

        public ICommandingDependencyResolverAdapter RegisterInstance<TType>(TType instance)
        {
            _serviceCollection.AddSingleton(typeof(TType), instance);
            return this;
        }

        public ICommandingDependencyResolverAdapter TypeMapping<TType, TImpl>() where TImpl : TType
        {
            _serviceCollection.AddTransient(typeof(TType), typeof(TImpl));
            return this;
        }

        public ICommandingDependencyResolverAdapter TypeMapping(Type type, Type impl)
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
