using System;
using System.Collections.Generic;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Implementation;

namespace AzureFromTheTrenches.Commanding
{
    /// <summary>
    /// The commanding runtime allows an instance of the commanding runtime to be registered within
    /// a IoC container.
    /// 
    /// Generally the CommandingDependencies methods should be used for this as that allows for slightly
    /// simpler registration semantics however if you want an isolated commanding runtime to be configured
    /// on a per thread basis with different options (rather than shared) this class should be used.
    /// </summary>
    public class CommandingRuntime
    {
        private ICommandRegistry _registry;
        private readonly object _registryLockObject = new object();
        private ICommandDispatchContextEnrichment _dispatchContextEnrichment;
        private readonly object _enrichmentLockObject = new object();
        private IAuditItemEnricherPipeline _auditItemEnricherPipeline;
        private readonly object _auditItemEnricherPipelineLockObject = new object();
        private ICommandAuditPipeline _auditorPipeline;
        private readonly object _auditorPipelineLockObject = new object();

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
        public ICommandRegistry UseCommanding(ICommandingDependencyResolver dependencyResolver,
            Action<Type> commandHandlerContainerRegistration)
        {
            return UseCommanding(dependencyResolver,
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
        public ICommandRegistry UseCommanding(ICommandingDependencyResolver dependencyResolver,
            Options options = null)
        {
            options = options ?? new Options();

            ICommandHandlerExecuter commandHandlerExecuter = new CommandHandlerExecuter();
            dependencyResolver.RegisterInstance(commandHandlerExecuter);
            IOptionsProvider optionsProvider = new OptionsProvider(options);
            dependencyResolver.RegisterInstance(optionsProvider);

            // the registry is always shared, but vagaries of different IoC containers mean its dangerous to rely
            // on dependecy resolver checks for an existing registration
            lock (_registryLockObject)
            {
                if (_registry == null || options.Reset)
                {
                    Action<Type> resolverContainerRegistration = type => dependencyResolver.TypeMapping(type, type);
                    _registry = new CommandRegistry(commandHandlerExecuter, options.CommandHandlerContainerRegistration ?? resolverContainerRegistration);
                }
                dependencyResolver.RegisterInstance(_registry);
            }

            // the enricher is always shared, but vagaries of different IoC containers mean its dangerous to rely
            // on dependecy resolver checks for an existing registration
            lock (_enrichmentLockObject)
            {
                if (_dispatchContextEnrichment == null || options.Reset)
                {
                    _dispatchContextEnrichment = new CommandDispatchContextEnrichment(options.Enrichers ?? new List<ICommandDispatchContextEnricher>());
                }
                else if (options.Enrichers != null)
                {
                    _dispatchContextEnrichment.AddEnrichers(options.Enrichers);
                }
                dependencyResolver.RegisterInstance(_dispatchContextEnrichment);
            }

            lock (_auditItemEnricherPipelineLockObject)
            {
                if (_auditItemEnricherPipeline == null || options.Reset)
                {
                    _auditItemEnricherPipeline = new AuditItemEnricherPipeline(
                        options.AuditItemEnricherFactoryFunc ?? (type => (IAuditItemEnricher)dependencyResolver.Resolve(type)));
                }
                dependencyResolver.RegisterInstance(_auditItemEnricherPipeline);
            }

            lock (_auditorPipelineLockObject)
            {
                if (_auditorPipeline == null || options.Reset)
                {
                    _auditorPipeline = new CommandAuditPipeline(t => (ICommandAuditor)dependencyResolver.Resolve(t),
                        dependencyResolver.Resolve<ICommandAuditSerializer>,
                        _auditItemEnricherPipeline);
                }
                dependencyResolver.RegisterInstance(_auditorPipeline);
            }

            ICommandHandlerFactory commandHandlerFactory = new CommandHandlerFactory(options.CommandHandlerFactoryFunc ?? dependencyResolver.Resolve);

            IPipelineAwareCommandHandlerExecuter pipelineAwareCommandHandlerExecuter = new PipelineAwareCommandHandlerExecuter();

            dependencyResolver.RegisterInstance(commandHandlerFactory);
            dependencyResolver.RegisterInstance(pipelineAwareCommandHandlerExecuter);

            dependencyResolver.TypeMapping<ICommandAuditorFactory, NullCommandAuditorFactory>();
            dependencyResolver.TypeMapping<ICommandScopeManager, AsyncLocalCommandScopeManager>();
            dependencyResolver.TypeMapping<IFrameworkCommandDispatcher, CommandDispatcher>();
            dependencyResolver.TypeMapping<ICommandDispatcher, CommandDispatcher>();
            dependencyResolver.TypeMapping<IFrameworkCommandExecuter, CommandExecuter>();
            dependencyResolver.TypeMapping<ICommandExecuter, CommandExecuter>();
            dependencyResolver.TypeMapping<IDirectCommandExecuter, DirectCommandExecuter>();
            if (options.DisableCorrelationIds)
            {
                dependencyResolver.TypeMapping<ICommandCorrelationIdProvider, DisabledCorrelationIdProvider>();
            }
            else
            {
                if (options.UseLocallyUniqueCorrelationIds)
                {
                    dependencyResolver
                        .TypeMapping<ICommandCorrelationIdProvider, LocallyUniqueCommandCorrelationIdProvider>();
                }
                else
                {
                    dependencyResolver.TypeMapping<ICommandCorrelationIdProvider, CommandCorrelationIdProvider>();
                }
            }

            dependencyResolver.TypeMapping<ICommandAuditSerializer, CommandAuditSerializer>();
            dependencyResolver.TypeMapping(typeof(ICommandExecutionExceptionHandler), options.CommandExecutionExceptionHandler ?? typeof(DefaultCommandExecutionExceptionHandler));

            return _registry;
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
        public ICommandingDependencyResolver UsePreDispatchCommandingAuditor<TDispatchAuditorImpl>(
            ICommandingDependencyResolver dependencyResolver, bool auditRootCommandOnly = true) where TDispatchAuditorImpl : ICommandAuditor
        {
            lock (_auditorPipelineLockObject)
            {
                if (_auditorPipeline == null)
                {
                    throw new AuditConfigurationException("The commanding system must be initialised with the UseCommanding method before any registering any auditors");
                }
                IAuditorRegistration registration = (IAuditorRegistration)_auditorPipeline;
                registration.RegisterPreDispatchAuditor<TDispatchAuditorImpl>(auditRootCommandOnly);
            }
            dependencyResolver.TypeMapping<TDispatchAuditorImpl, TDispatchAuditorImpl>();
            return dependencyResolver;
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
        public ICommandingDependencyResolver UsePostDispatchCommandingAuditor<TDispatchAuditorImpl>(
            ICommandingDependencyResolver dependencyResolver, bool auditRootCommandOnly = true) where TDispatchAuditorImpl : ICommandAuditor
        {
            lock (_auditorPipelineLockObject)
            {
                if (_auditorPipeline == null)
                {
                    throw new AuditConfigurationException("The commanding system must be initialised with the UseCommanding method before any registering any auditors");
                }
                IAuditorRegistration registration = (IAuditorRegistration)_auditorPipeline;
                registration.RegisterPostDispatchAuditor<TDispatchAuditorImpl>(auditRootCommandOnly);
            }
            dependencyResolver.TypeMapping<TDispatchAuditorImpl, TDispatchAuditorImpl>();
            return dependencyResolver;
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
        public ICommandingDependencyResolver UseExecutionCommandingAuditor<TExecutionAuditorImpl>(
            ICommandingDependencyResolver dependencyResolver, bool auditRootCommandOnly = true) where TExecutionAuditorImpl : ICommandAuditor
        {
            lock (_auditorPipelineLockObject)
            {
                if (_auditorPipeline == null)
                {
                    throw new AuditConfigurationException("The commanding system must be initialised with the UseCommanding method before any registering any auditors");
                }
                IAuditorRegistration registration = (IAuditorRegistration)_auditorPipeline;
                registration.RegisterExecutionAuditor<TExecutionAuditorImpl>(auditRootCommandOnly);
            }
            dependencyResolver.TypeMapping<TExecutionAuditorImpl, TExecutionAuditorImpl>();
            return dependencyResolver;
        }

        public ICommandingDependencyResolver UseAuditItemEnricher<TAuditItemEnricher>(ICommandingDependencyResolver commandingDependencyResolver)
            where TAuditItemEnricher : IAuditItemEnricher
        {
            lock (_auditItemEnricherPipelineLockObject)
            {
                if (_auditItemEnricherPipeline == null)
                {
                    throw new AuditConfigurationException("The commanding system must be initialised with the UseCommanding method before any registering any audit item enrichers");
                }
                _auditItemEnricherPipeline.AddEnricher<TAuditItemEnricher>();
            }
            commandingDependencyResolver.TypeMapping<TAuditItemEnricher, TAuditItemEnricher>();
            return commandingDependencyResolver;
        }
    }
}
