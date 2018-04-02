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
                resolver.UsePreDispatchCommandingAuditor<LoggerCommandAuditor>(options.AuditPreDispatchRootOnly);
            }
            if (options.UsePostDispatchAuditor)
            {
                resolver.UsePostDispatchCommandingAuditor<LoggerCommandAuditor>(options.AuditPostDispatchRootOnly);
            }
            if (options.UsePreDispatchAuditor)
            {
                resolver.UseExecutionCommandingAuditor<LoggerCommandAuditor>(options.AuditExecuteDispatchRootOnly);
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
                resolver.AddPreDispatchCommandingAuditor<LoggerCommandAuditor>(options.AuditPreDispatchRootOnly);
            }
            if (options.UsePostDispatchAuditor)
            {
                resolver.AddPostDispatchCommandingAuditor<LoggerCommandAuditor>(options.AuditPostDispatchRootOnly);
            }
            if (options.UsePreDispatchAuditor)
            {
                resolver.AddExecutionCommandingAuditor<LoggerCommandAuditor>(options.AuditExecuteDispatchRootOnly);
            }

            return resolver;
        }
    }
}
