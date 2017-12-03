using System;
using Microsoft.Extensions.DependencyInjection;

namespace AzureFromTheTrenches.Commanding.MicrosoftDependencyInjection
{
    // ReSharper disable once InconsistentNaming
    public static class IServiceCollectionExtensions
    {
        public static IMicrosoftDependencyInjectionCommandingResolver UseCommanding(this IServiceCollection serviceCollection,
            Options options=null)
        {
            MicrosoftDependencyInjectionCommandingResolver resolver = new MicrosoftDependencyInjectionCommandingResolver(serviceCollection);
            resolver.Registry = resolver.UseCommanding(options);
            return resolver;
        }
    }
}
