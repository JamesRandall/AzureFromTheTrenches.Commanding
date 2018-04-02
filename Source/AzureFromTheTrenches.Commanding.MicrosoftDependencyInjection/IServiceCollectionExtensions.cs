using System;
using Microsoft.Extensions.DependencyInjection;

namespace AzureFromTheTrenches.Commanding.MicrosoftDependencyInjection
{
    // ReSharper disable once InconsistentNaming
    public static class IServiceCollectionExtensions
    {
        [Obsolete]
        public static IMicrosoftDependencyInjectionCommandingResolver UseCommanding(this IServiceCollection serviceCollection,
            Options options=null)
        {
            MicrosoftDependencyInjectionCommandingResolver resolver = new MicrosoftDependencyInjectionCommandingResolver(serviceCollection);
            resolver.Registry = resolver.UseCommanding(options);
            return resolver;
        }

        public static IMicrosoftDependencyInjectionCommandingResolverAdapter AddCommanding(this IServiceCollection serviceCollection,
            Options options = null)
        {
            MicrosoftDependencyInjectionCommandingResolverAdapter resolver = new MicrosoftDependencyInjectionCommandingResolverAdapter(serviceCollection);
            resolver.Registry = resolver.AddCommanding(options);
            return resolver;
        }
    }
}
