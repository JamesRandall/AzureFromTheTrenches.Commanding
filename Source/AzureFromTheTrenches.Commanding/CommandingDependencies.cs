using System;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding
{
    public static class CommandingDependencies
    {
        private static readonly CommandingRuntime Instance = new CommandingRuntime();
        
        /// <summary>
        /// Registers the commanding system in an ioc container.
        /// If the container is not able to resolve unregistered types (for example the NetStandard Microsoft container) then
        /// the commandHandlerContainerRegistration should be used to perform the type registration for the handler
        /// </summary>
        /// <param name="dependencyResolver">The dependency resolver to register inside</param>
        /// <param name="commandHandlerContainerRegistration">
        /// Unless an alternative implementation of ICommandHandlerFactory is supplied then actors are created through the dependency resolver
        /// but not all IoC containers can resolve unregistered concrete types (for example the built in ASP.Net Core IServiceCollection
        /// and IServiceProvider IoC cannot). Where this is the case supply an implementation for the CommandHandlerContainerRegistration
        /// action that registers the actors in the container. For example using an IServiceCollection instance of serviceCollection:
        ///     resolver.UseCommanding(type => services.AddTransient(type, type));
        /// </param>
        /// <returns>The dependency resolver</returns>
        [Obsolete("This will be removed in a future version, please use AddCommanding instead")]
        public static ICommandRegistry UseCommanding(this ICommandingDependencyResolver dependencyResolver,
            Action<Type> commandHandlerContainerRegistration)
        {
            return Instance.UseCommanding(dependencyResolver,
                new Options {CommandHandlerContainerRegistration = commandHandlerContainerRegistration});
        }

        /// <summary>
        /// Registers the commanding system in an ioc container.
        /// If the container is not able to resolve unregistered types (for example the NetStandard Microsoft container) then
        /// the commandHandlerContainerRegistration should be used to perform the type registration for the handler
        /// </summary>
        /// <param name="dependencyResolver">The dependency resolver to register inside</param>
        /// <param name="commandHandlerContainerRegistration">
        /// Unless an alternative implementation of ICommandHandlerFactory is supplied then actors are created through the dependency resolver
        /// but not all IoC containers can resolve unregistered concrete types (for example the built in ASP.Net Core IServiceCollection
        /// and IServiceProvider IoC cannot). Where this is the case supply an implementation for the CommandHandlerContainerRegistration
        /// action that registers the actors in the container. For example using an IServiceCollection instance of serviceCollection:
        ///     resolver.UseCommanding(type => services.AddTransient(type, type));
        /// </param>
        /// <returns>The dependency resolver</returns>
        public static ICommandRegistry AddCommanding(this ICommandingDependencyResolverAdapter dependencyResolver,
            Action<Type> commandHandlerContainerRegistration)
        {
            return Instance.AddCommanding(dependencyResolver,
                new Options { CommandHandlerContainerRegistration = commandHandlerContainerRegistration });
        }

        /// <summary>
        /// Registers the commanding system in an ioc container.
        /// If the container is not able to resolve unregistered types (for example the NetStandard Microsoft container) then
        /// the commandHandlerContainerRegistration should be used to perform the type registration for the handler
        /// </summary>
        /// <param name="dependencyResolver">The dependency resolver to register inside</param>
        /// <param name="options">Configuration options for the commanding system</param>
        /// <returns>The dependency resolver</returns>
        [Obsolete("Please use AddCommanding instead")]
        public static ICommandRegistry UseCommanding(this ICommandingDependencyResolver dependencyResolver,
            Options options = null)
        {
            return Instance.UseCommanding(dependencyResolver, options);
        }

        /// <summary>
        /// Registers the commanding system in an ioc container.
        /// If the container is not able to resolve unregistered types (for example the NetStandard Microsoft container) then
        /// the commandHandlerContainerRegistration should be used to perform the type registration for the handler
        /// </summary>
        /// <param name="dependencyResolver">The dependency resolver to register inside</param>
        /// <param name="options">Configuration options for the commanding system</param>
        /// <returns>The dependency resolver</returns>
        public static ICommandRegistry AddCommanding(this ICommandingDependencyResolverAdapter dependencyResolver,
            Options options = null)
        {
            return Instance.AddCommanding(dependencyResolver, options);
        }

        /// <summary>
        /// Registers an auditor that will be invoked directly before a command has been dispatched.
        /// </summary>
        /// <typeparam name="TDispatchAuditorImpl">The type of the auditor</typeparam>
        /// <param name="dependencyResolver">The dependency resolver</param>
        /// <param name="auditRootCommandOnly">By default the built in auditor will audit every command that is dispatched however if using the audit as part of an
        /// event sourcing pipeline it can be useful to only audit the root command and exclude any commands dispatched as a result
        /// of that root command. Set this property to true to audit only the root commands, leave null or set to false to audit all
        /// commands.</param>
        /// <returns>The dependency resolver</returns>
        [Obsolete("Please use AddPreDispatchCommandingAuditor instead")]
        public static ICommandingDependencyResolver UsePreDispatchCommandingAuditor<TDispatchAuditorImpl>(
            this ICommandingDependencyResolver dependencyResolver, bool auditRootCommandOnly=true) where TDispatchAuditorImpl : ICommandAuditor
        {
            return Instance.UsePreDispatchCommandingAuditor<TDispatchAuditorImpl>(dependencyResolver, auditRootCommandOnly);
        }

        /// <summary>
        /// Registers an auditor that will be invoked directly before a command has been dispatched.
        /// </summary>
        /// <typeparam name="TDispatchAuditorImpl">The type of the auditor</typeparam>
        /// <param name="dependencyResolver">The dependency resolver</param>
        /// <param name="auditRootCommandOnly">By default the built in auditor will audit every command that is dispatched however if using the audit as part of an
        /// event sourcing pipeline it can be useful to only audit the root command and exclude any commands dispatched as a result
        /// of that root command. Set this property to true to audit only the root commands, leave null or set to false to audit all
        /// commands.</param>
        /// <returns>The dependency resolver</returns>
        public static ICommandingDependencyResolverAdapter AddPreDispatchCommandingAuditor<TDispatchAuditorImpl>(
            this ICommandingDependencyResolverAdapter dependencyResolver, bool auditRootCommandOnly = true) where TDispatchAuditorImpl : ICommandAuditor
        {
            return Instance.AddPreDispatchCommandingAuditor<TDispatchAuditorImpl>(dependencyResolver, auditRootCommandOnly);
        }

        /// <summary>
        /// Registers an auditor that will be invoked directly after a command has been dispatched.
        /// </summary>
        /// <typeparam name="TDispatchAuditorImpl">The type of the auditor</typeparam>
        /// <param name="dependencyResolver">The dependency resolver</param>
        /// <param name="auditRootCommandOnly">By default the built in auditor will audit every command that is dispatched however if using the audit as part of an
        /// event sourcing pipeline it can be useful to only audit the root command and exclude any commands dispatched as a result
        /// of that root command. Set this property to true to audit only the root commands, leave null or set to false to audit all
        /// commands.</param>
        /// <returns>The dependency resolver</returns>
        [Obsolete("Please use AddPostDispatchCommandingAuditor instead")]
        public static ICommandingDependencyResolver UsePostDispatchCommandingAuditor<TDispatchAuditorImpl>(
            this ICommandingDependencyResolver dependencyResolver, bool auditRootCommandOnly = true) where TDispatchAuditorImpl : ICommandAuditor
        {
            return Instance.UsePostDispatchCommandingAuditor<TDispatchAuditorImpl>(dependencyResolver, auditRootCommandOnly);
        }

        /// <summary>
        /// Registers an auditor that will be invoked directly after a command has been dispatched.
        /// </summary>
        /// <typeparam name="TDispatchAuditorImpl">The type of the auditor</typeparam>
        /// <param name="dependencyResolver">The dependency resolver</param>
        /// <param name="auditRootCommandOnly">By default the built in auditor will audit every command that is dispatched however if using the audit as part of an
        /// event sourcing pipeline it can be useful to only audit the root command and exclude any commands dispatched as a result
        /// of that root command. Set this property to true to audit only the root commands, leave null or set to false to audit all
        /// commands.</param>
        /// <returns>The dependency resolver</returns>
        public static ICommandingDependencyResolverAdapter AddPostDispatchCommandingAuditor<TDispatchAuditorImpl>(
            this ICommandingDependencyResolverAdapter dependencyResolver, bool auditRootCommandOnly = true) where TDispatchAuditorImpl : ICommandAuditor
        {
            return Instance.AddPostDispatchCommandingAuditor<TDispatchAuditorImpl>(dependencyResolver, auditRootCommandOnly);
        }

        /// <summary>
        /// Registers an auditor that will be invoked directly after a command has been executed.
        /// </summary>
        /// <typeparam name="TExecutionAuditorImpl">The type of the auditor</typeparam>
        /// <param name="dependencyResolver">The dependency resolver</param>
        /// <param name="auditRootCommandOnly">By default the built in auditor will audit every command that is dispatched however if using the audit as part of an
        /// event sourcing pipeline it can be useful to only audit the root command and exclude any commands dispatched as a result
        /// of that root command. Set this property to true to audit only the root commands, leave null or set to false to audit all
        /// commands.</param>
        /// <returns>The dependency resolver</returns>
        [Obsolete("Please use  instead")]
        public static ICommandingDependencyResolver UseExecutionCommandingAuditor<TExecutionAuditorImpl>(
            this ICommandingDependencyResolver dependencyResolver, bool auditRootCommandOnly=true) where TExecutionAuditorImpl : ICommandAuditor
        {
            return Instance.UseExecutionCommandingAuditor<TExecutionAuditorImpl>(dependencyResolver, auditRootCommandOnly);
        }

        /// <summary>
        /// Registers an auditor that will be invoked directly after a command has been executed.
        /// </summary>
        /// <typeparam name="TExecutionAuditorImpl">The type of the auditor</typeparam>
        /// <param name="dependencyResolver">The dependency resolver</param>
        /// <param name="auditRootCommandOnly">By default the built in auditor will audit every command that is dispatched however if using the audit as part of an
        /// event sourcing pipeline it can be useful to only audit the root command and exclude any commands dispatched as a result
        /// of that root command. Set this property to true to audit only the root commands, leave null or set to false to audit all
        /// commands.</param>
        /// <returns>The dependency resolver</returns>
        public static ICommandingDependencyResolverAdapter AddExecutionCommandingAuditor<TExecutionAuditorImpl>(
            this ICommandingDependencyResolverAdapter dependencyResolver, bool auditRootCommandOnly = true) where TExecutionAuditorImpl : ICommandAuditor
        {
            return Instance.AddExecutionCommandingAuditor<TExecutionAuditorImpl>(dependencyResolver, auditRootCommandOnly);
        }

        /// <summary>
        /// Adds an audit item enricher
        /// </summary>
        /// <typeparam name="TAuditItemEnricher">The type of the enricher</typeparam>
        /// <param name="commandingDependencyResolver">The commanding dependency resolver</param>
        /// <returns>The commanding dependency resolver</returns>
        [Obsolete("Please use AddAuditItemEnricher instead")]
        public static ICommandingDependencyResolver UseAuditItemEnricher<TAuditItemEnricher>(this ICommandingDependencyResolver commandingDependencyResolver)
            where TAuditItemEnricher : IAuditItemEnricher
        {
            return Instance.UseAuditItemEnricher<TAuditItemEnricher>(commandingDependencyResolver);
        }

        /// <summary>
        /// Adds an audit item enricher
        /// </summary>
        /// <typeparam name="TAuditItemEnricher">The type of the enricher</typeparam>
        /// <param name="commandingDependencyResolver">The commanding dependency resolver</param>
        /// <returns>The commanding dependency resolver</returns>
        public static ICommandingDependencyResolverAdapter AddAuditItemEnricher<TAuditItemEnricher>(this ICommandingDependencyResolverAdapter commandingDependencyResolver)
            where TAuditItemEnricher : IAuditItemEnricher
        {
            return Instance.AddAuditItemEnricher<TAuditItemEnricher>(commandingDependencyResolver);
        }
    }
}
