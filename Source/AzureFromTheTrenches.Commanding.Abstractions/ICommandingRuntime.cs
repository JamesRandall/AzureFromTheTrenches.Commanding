using System;

namespace AzureFromTheTrenches.Commanding.Abstractions
{
    public interface ICommandingRuntime
    {
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
        [Obsolete("Please use AddCommanding instead")]
        ICommandRegistry UseCommanding(ICommandingDependencyResolver dependencyResolver,
            Action<Type> commandHandlerContainerRegistration);

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
        ICommandRegistry AddCommanding(ICommandingDependencyResolverAdapter dependencyResolver,
            Action<Type> commandHandlerContainerRegistration);

        /// <summary>
        /// Registers the commanding system in an ioc container.
        /// If the container is not able to resolve unregistered types (for example the NetStandard Microsoft container) then
        /// the commandHandlerContainerRegistration should be used to perform the type registration for the handler
        /// </summary>
        /// <param name="dependencyResolver">The dependency resolver to register inside</param>
        /// <param name="options">Configuration options for the commanding system</param>
        /// <returns>The dependency resolver</returns>
        [Obsolete("Please use AddCommanding instead")]
        ICommandRegistry UseCommanding(ICommandingDependencyResolver dependencyResolver,
            IOptions options = null);

        /// <summary>
        /// Registers the commanding system in an ioc container.
        /// If the container is not able to resolve unregistered types (for example the NetStandard Microsoft container) then
        /// the commandHandlerContainerRegistration should be used to perform the type registration for the handler
        /// </summary>
        /// <param name="dependencyResolver">The dependency resolver to register inside</param>
        /// <param name="options">Configuration options for the commanding system</param>
        /// <returns>The dependency resolver</returns>
        ICommandRegistry AddCommanding(ICommandingDependencyResolverAdapter dependencyResolver,
            IOptions options = null);

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
        ICommandingDependencyResolver UsePreDispatchCommandingAuditor<TDispatchAuditorImpl>(
            ICommandingDependencyResolver dependencyResolver, bool auditRootCommandOnly = true) where TDispatchAuditorImpl : ICommandAuditor;

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
        ICommandingDependencyResolverAdapter AddPreDispatchCommandingAuditor<TDispatchAuditorImpl>(
            ICommandingDependencyResolverAdapter dependencyResolver, bool auditRootCommandOnly = true) where TDispatchAuditorImpl : ICommandAuditor;

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
        ICommandingDependencyResolver UsePostDispatchCommandingAuditor<TDispatchAuditorImpl>(
            ICommandingDependencyResolver dependencyResolver, bool auditRootCommandOnly = true) where TDispatchAuditorImpl : ICommandAuditor;

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
        ICommandingDependencyResolverAdapter AddPostDispatchCommandingAuditor<TDispatchAuditorImpl>(
            ICommandingDependencyResolverAdapter dependencyResolver, bool auditRootCommandOnly = true) where TDispatchAuditorImpl : ICommandAuditor;

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
        [Obsolete("Please use AddExecutionCommandingAuditor instead")]
        ICommandingDependencyResolver UseExecutionCommandingAuditor<TExecutionAuditorImpl>(
            ICommandingDependencyResolver dependencyResolver, bool auditRootCommandOnly = true) where TExecutionAuditorImpl : ICommandAuditor;

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
        ICommandingDependencyResolverAdapter AddExecutionCommandingAuditor<TExecutionAuditorImpl>(
            ICommandingDependencyResolverAdapter dependencyResolver, bool auditRootCommandOnly = true) where TExecutionAuditorImpl : ICommandAuditor;

        /// <summary>
        /// Registers a type that will add additional properties to audit items
        /// </summary>
        /// <typeparam name="TAuditItemEnricher">Type of the enricher</typeparam>
        /// <param name="commandingDependencyResolver">The dependency resolver</param>
        /// <returns>The dependency resolver</returns>
        [Obsolete("Please use AddAuditItemEnricher instead")]
        ICommandingDependencyResolver UseAuditItemEnricher<TAuditItemEnricher>(ICommandingDependencyResolver commandingDependencyResolver)
            where TAuditItemEnricher : IAuditItemEnricher;

        /// <summary>
        /// Adds an audit item enricher
        /// </summary>
        /// <typeparam name="TAuditItemEnricher">The type of the enricher</typeparam>
        /// <param name="commandingDependencyResolver">The commanding dependency resolver</param>
        /// <returns>The commanding dependency resolver</returns>
        ICommandingDependencyResolverAdapter AddAuditItemEnricher<TAuditItemEnricher>(ICommandingDependencyResolverAdapter commandingDependencyResolver)
            where TAuditItemEnricher : IAuditItemEnricher;
    }
}