using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.MicrosoftLoggingExtensions.Implementation;
using Microsoft.Extensions.Logging;

namespace AzureFromTheTrenches.Commanding.MicrosoftLoggingExtensions
{
    public static class MicrosoftLoggingExtensionsDependencies
    {
        [Obsolete("Pleasee use AddMicrosoftLoggingExtensionsAuditor instead")]
        public static ICommandingDependencyResolver UseMicrosoftLoggingExtensionsAuditor(this ICommandingDependencyResolver resolver,
            LogLevel normalLogLevel = LogLevel.Trace,
            LogLevel executionFailureLogLevel = LogLevel.Warning,
            MicrosoftLoggingExtensionsAuditorOptions options = null)
        {
            options = options ?? new MicrosoftLoggingExtensionsAuditorOptions();
            ILogLevelProvider logLevelProvider = new LogLevelProvider(normalLogLevel, executionFailureLogLevel);
            resolver.RegisterInstance(logLevelProvider);

            if (options.UsePreDispatchAuditor)
            {
                EnsureCommandingRuntime(resolver);
                resolver.AssociatedCommandingRuntime.UsePreDispatchCommandingAuditor<LoggerCommandAuditor>(resolver, options.AuditPreDispatchRootOnly);
            }
            if (options.UsePostDispatchAuditor)
            {
                EnsureCommandingRuntime(resolver);
                resolver.AssociatedCommandingRuntime.UsePostDispatchCommandingAuditor<LoggerCommandAuditor>(resolver, options.AuditPostDispatchRootOnly);
            }
            if (options.UsePreDispatchAuditor)
            {
                EnsureCommandingRuntime(resolver);
                resolver.AssociatedCommandingRuntime.UseExecutionCommandingAuditor<LoggerCommandAuditor>(resolver, options.AuditExecuteDispatchRootOnly);
            }

            return resolver;
        }

        public static ICommandingDependencyResolverAdapter AddMicrosoftLoggingExtensionsAuditor(this ICommandingDependencyResolverAdapter resolver,
            LogLevel normalLogLevel = LogLevel.Trace,
            LogLevel executionFailureLogLevel = LogLevel.Warning,
            MicrosoftLoggingExtensionsAuditorOptions options = null)
        {
            options = options ?? new MicrosoftLoggingExtensionsAuditorOptions();
            ILogLevelProvider logLevelProvider = new LogLevelProvider(normalLogLevel, executionFailureLogLevel);
            resolver.RegisterInstance(logLevelProvider);

            if (options.UsePreDispatchAuditor)
            {
                EnsureCommandingRuntime(resolver);
                resolver.AssociatedCommandingRuntime.AddPreDispatchCommandingAuditor<LoggerCommandAuditor>(resolver, options.AuditPreDispatchRootOnly);
            }
            if (options.UsePostDispatchAuditor)
            {
                EnsureCommandingRuntime(resolver);
                resolver.AssociatedCommandingRuntime.AddPostDispatchCommandingAuditor<LoggerCommandAuditor>(resolver, options.AuditPostDispatchRootOnly);
            }
            if (options.UsePreDispatchAuditor)
            {
                EnsureCommandingRuntime(resolver);
                resolver.AssociatedCommandingRuntime.AddExecutionCommandingAuditor<LoggerCommandAuditor>(resolver, options.AuditExecuteDispatchRootOnly);
            }

            return resolver;
        }

        [Obsolete]
        private static void EnsureCommandingRuntime(ICommandingDependencyResolver dependencyResolver)
        {
            if (dependencyResolver.AssociatedCommandingRuntime == null)
            {
                throw new CommandFrameworkConfigurationException("The commanding package should be configured first");
            }
        }

        private static void EnsureCommandingRuntime(ICommandingDependencyResolverAdapter dependencyResolver)
        {
            if (dependencyResolver.AssociatedCommandingRuntime == null)
            {
                throw new CommandFrameworkConfigurationException("The commanding package should be configured first");
            }
        }
    }
}
