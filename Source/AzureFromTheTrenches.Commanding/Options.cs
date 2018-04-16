using System;
using System.Collections.Generic;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding
{
    /// <summary>
    /// Options for configuring the command system
    /// </summary>
    public class Options : IOptions
    {
        /// <summary>
        /// Sets defaults
        /// </summary>
        public Options()
        {
            AuditItemEnricherFactoryFunc = null;
            CommandHandlerContainerRegistration = null;
            CommandHandlerFactoryFunc = null;
            CommandExecutionExceptionHandler = null;
            MetricCollectionEnabled = true;
            Reset = false;
            Enrichers = null;
        }

        /// <summary>
        /// Can be used to register a type that implements the ICommandExecutionExceptionHandler interface that will be
        /// invoked in the event of an execution error
        /// </summary>
        public Type CommandExecutionExceptionHandler { get; set; }
        /// <summary>
        /// Setting this to true will disable all correlatin ID generation which can give a small performance boost
        /// in super-low latency situations.
        /// </summary>
        public bool DisableCorrelationIds { get; set; }
        /// <summary>
        /// Unless an alternative implementation of ICommandHandlerFactory is supplied or the CommandHandlerFactoryFunc in this options
        /// class is set then actors are created through the dependency resolver but not all IoC containers can resolve unregistered concrete
        /// types (for example the built in ASP.Net Core IServiceCollection and IServiceProvider IoC cannot). Where this is the case
        /// supply an implementation for the CommandHandlerContainerRegistration action that registers the actors in the container. For
        /// example using an IServiceCollection instance of serviceCollection:
        ///     var options = new Options() { CommandHandlerContainerRegistration = type => services.AddTransient(type, type) };
        /// </summary>
        public Action<Type> CommandHandlerContainerRegistration { get; set; }
        /// <summary>
        /// By default actors are created through the dependency resolver but if a function is assigned to the CommandHandlerFactoryFunc
        /// property then that function will be called to instantiate an handler.
        /// </summary>
        public Func<Type, object> CommandHandlerFactoryFunc { get; set; }
        /// <summary>
        /// By default audit item enrichers are created through the dependency resolver but if a function is assigned to the
        /// AuditItemEnricherFactoryFunc property then that function will be used to instantiate an enricher
        /// </summary>
        public Func<Type, IAuditItemEnricher> AuditItemEnricherFactoryFunc { get; set; }
        /// <summary>
        /// The commanding system maintains command registrations, command context enrichers, and auditors between calls to UseCommanding
        /// to enable sub-modules to extend the system without any awareness / tight coupling between them. If you want to force a reset
        /// of the shared state then set this property to true (generally this is only useful in sample apps where you might repeatedly
        /// recreate and reconfigure an IoC container as the included samples do.
        /// </summary>
        public bool Reset { get; set; }
        /// <summary>
        /// It can be useful to include additional properties in the command context, for example the Application Insights Operation ID.
        /// This can be done by setting this property to a set of enrichment objects - these are called in sequence and
        /// are passed the current state of the command contexts enriched properties and can return a dictionary of properties to insert.
        /// If the returned dictionary contains a property that already exists it will be replaced in the command context property bag.
        /// </summary>
        public IEnumerable<ICommandDispatchContextEnricher> Enrichers { get; set; }
        /// <summary>
        /// Set to true to gather timing metrics around command dispatch and execution. Note this has a minor performance
        /// implication in high throughput environments.
        /// </summary>
        public bool MetricCollectionEnabled { get; set; }
        /// <summary>
        /// By default correlation IDs will be generated as GUIDs so that they are unique across as distributed system however
        /// if performance is key a local;y unique (incrementing long, thread safe) correlation ID can be used by setting this to true
        /// </summary>
        public bool UseLocallyUniqueCorrelationIds { get; set; }
    }
}
