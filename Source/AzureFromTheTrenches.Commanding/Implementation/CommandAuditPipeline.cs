using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;
using AzureFromTheTrenches.Commanding.Model;

namespace AzureFromTheTrenches.Commanding.Implementation
{
    internal class CommandAuditPipeline : ICommandAuditPipeline, IAuditorRegistration
    {
        private const string PreDispatchType = "predispatch";
        private const string PostDispatchType = "postdispatch";
        private const string ExecutionType = "execution";

        private readonly Func<Type, ICommandAuditor> _auditorFactoryFunc;
        private readonly Func<ICommandAuditSerializer> _auditorSerializerFunc;
        private readonly List<AuditorDefinition> _registeredPreDispatchAuditors = new List<AuditorDefinition>();
        private readonly List<AuditorDefinition> _registeredPostDispatchAuditors = new List<AuditorDefinition>();
        private readonly List<AuditorDefinition> _registeredExecutionAuditors = new List<AuditorDefinition>();
        private volatile IReadOnlyCollection<AuditorInstance> _preDispatchAuditors;
        private volatile IReadOnlyCollection<AuditorInstance> _postDispatchAuditors;
        private volatile IReadOnlyCollection<AuditorInstance> _executionAuditors;
        private readonly object _dispatchAuditorCreationLock = new object();
        private readonly object _executionAuditorCreationLock = new object();

        public CommandAuditPipeline(Func<Type, ICommandAuditor> auditorFactoryFunc, Func<ICommandAuditSerializer> auditorSerializerFunc)
        {
            _auditorFactoryFunc = auditorFactoryFunc;
            _auditorSerializerFunc = auditorSerializerFunc;
        }

        
        public void RegisterPreDispatchAuditor<TAuditorImpl>(bool auditRootCommandOnly) where TAuditorImpl : ICommandAuditor
        {
            // all auditors must be registered before the first command is dispatched
            _registeredPreDispatchAuditors.Add(new AuditorDefinition(typeof(TAuditorImpl), auditRootCommandOnly));
        }

        public void RegisterPostDispatchAuditor<TAuditorImpl>(bool auditRootCommandOnly) where TAuditorImpl : ICommandAuditor
        {
            // all auditors must be registered before the first command is dispatched
            _registeredPostDispatchAuditors.Add(new AuditorDefinition(typeof(TAuditorImpl), auditRootCommandOnly));
        }

        public void RegisterExecutionAuditor<TAuditorImpl>(bool auditRootCommandOnly) where TAuditorImpl : ICommandAuditor
        {
            // all auditors must be registered before the first command is dispatched
            _registeredExecutionAuditors.Add(new AuditorDefinition(typeof(TAuditorImpl), auditRootCommandOnly));
        }

        public async Task AuditPreDispatch(ICommand command, ICommandDispatchContext dispatchContext, CancellationToken cancellationToken)
        {
            ICommandAuditSerializer serializer = _auditorSerializerFunc();
            
            AuditItem auditItem = new AuditItem
            {
                AdditionalProperties = dispatchContext.AdditionalProperties.ToDictionary(x => x.Key, x => x.Value.ToString()),
                CommandId = null,
                CommandType = command.GetType().AssemblyQualifiedName,
                CorrelationId = dispatchContext.CorrelationId,
                Depth = dispatchContext.Depth,
                DispatchedUtc = DateTime.UtcNow,
                SerializedCommand = serializer.Serialize(command),
                Type = PreDispatchType
            };
            // ReSharper disable once SuspiciousTypeConversion.Global - used by consumers of the package
            if (command is IIdentifiableCommand identifiableCommand)
            {
                auditItem.CommandId = identifiableCommand.Id;
            }
            await AuditPreDispatch(auditItem, cancellationToken);
        }

        public async Task AuditPreDispatch(AuditItem auditItem, CancellationToken cancellationToken)
        {
            auditItem.Type = PreDispatchType;
            IReadOnlyCollection<ICommandAuditor> auditors = GetPreDispatchAuditors(auditItem.Depth == 0);
            List<Task> auditTasks = new List<Task>();
            foreach (ICommandAuditor auditor in auditors)
            {
                auditTasks.Add(auditor.Audit(auditItem, cancellationToken));
            }
            await Task.WhenAll(auditTasks);
        }

        public async Task AuditPostDispatch(ICommand command, ICommandDispatchContext dispatchContext, CancellationToken cancellationToken)
        {
            ICommandAuditSerializer serializer = _auditorSerializerFunc();

            AuditItem auditItem = new AuditItem
            {
                AdditionalProperties = dispatchContext.AdditionalProperties.ToDictionary(x => x.Key, x => x.Value.ToString()),
                CommandId = null,
                CommandType = command.GetType().AssemblyQualifiedName,
                CorrelationId = dispatchContext.CorrelationId,
                Depth = dispatchContext.Depth,
                DispatchedUtc = DateTime.UtcNow,
                SerializedCommand = serializer.Serialize(command),
                Type = PostDispatchType
            };
            // ReSharper disable once SuspiciousTypeConversion.Global - used by consumers of the package
            if (command is IIdentifiableCommand identifiableCommand)
            {
                auditItem.CommandId = identifiableCommand.Id;
            }
            await AuditPostDispatch(auditItem, cancellationToken);
        }

        public async Task AuditPostDispatch(AuditItem auditItem, CancellationToken cancellationToken)
        {
            auditItem.Type = PostDispatchType;
            IReadOnlyCollection<ICommandAuditor> auditors = GetPostDispatchAuditors(auditItem.Depth == 0);
            List<Task> auditTasks = new List<Task>();
            foreach (ICommandAuditor auditor in auditors)
            {
                auditTasks.Add(auditor.Audit(auditItem, cancellationToken));
            }
            await Task.WhenAll(auditTasks);
        }

        public async Task AuditExecution(ICommand command, ICommandDispatchContext dispatchContext,
            bool executedSuccessfully, CancellationToken cancellationToken)
        {
            ICommandAuditSerializer serializer = _auditorSerializerFunc();
            AuditItem auditItem = new AuditItem
            {
                AdditionalProperties = dispatchContext.AdditionalProperties.ToDictionary(x => x.Key, x => x.Value.ToString()),
                CommandId = null,
                CommandType = command.GetType().AssemblyQualifiedName,
                CorrelationId = dispatchContext.CorrelationId,
                Depth = dispatchContext.Depth,
                DispatchedUtc = DateTime.UtcNow,
                ExecutedSuccessfully = executedSuccessfully,
                SerializedCommand = serializer.Serialize(command),
                Type = ExecutionType
            };
            // ReSharper disable once SuspiciousTypeConversion.Global - used by consumers of the package
            if (command is IIdentifiableCommand identifiableCommand)
            {
                auditItem.CommandId = identifiableCommand.Id;
            }
            await AuditExecution(auditItem, cancellationToken);
        }

        public async Task AuditExecution(AuditItem auditItem, CancellationToken cancellationToken)
        {
            auditItem.Type = ExecutionType;
            IReadOnlyCollection<ICommandAuditor> auditors = GetExecutionAuditors(auditItem.Depth == 0);
            List<Task> auditTasks = new List<Task>();
            foreach (ICommandAuditor auditor in auditors)
            {
                auditTasks.Add(auditor.Audit(auditItem, cancellationToken));
            }
            await Task.WhenAll(auditTasks);
        }

        public async Task Audit(AuditItem auditItem, CancellationToken cancellationToken)
        {
            if (auditItem.Type == ExecutionType)
            {
                await AuditExecution(auditItem, cancellationToken);
            }
            else if (auditItem.Type == PostDispatchType)
            {
                await AuditPostDispatch(auditItem, cancellationToken);
            }
            else if (auditItem.Type == PreDispatchType)
            {
                await AuditPreDispatch(auditItem, cancellationToken);
            }
            else
            {
                throw new AuditorException($"Audit type of {auditItem.Type} is not recognised");
            }
        }

        private IReadOnlyCollection<ICommandAuditor> GetPreDispatchAuditors(bool isRootCommand)
        {
            if (_preDispatchAuditors == null)
            {
                lock (_dispatchAuditorCreationLock)
                {
                    if (_preDispatchAuditors == null)
                    {
                        _preDispatchAuditors = _registeredPreDispatchAuditors
                            .Select(x => new AuditorInstance(_auditorFactoryFunc(x.AuditorType), x.AuditRootCommandOnly)).ToArray();
                    }
                }
            }
            return _preDispatchAuditors.Where(x => isRootCommand || !x.AuditRootCommandOnly).Select(x => x.Auditor).ToArray();
        }

        private IReadOnlyCollection<ICommandAuditor> GetPostDispatchAuditors(bool isRootCommand)
        {
            if (_postDispatchAuditors == null)
            {
                lock (_dispatchAuditorCreationLock)
                {
                    if (_postDispatchAuditors == null)
                    {
                        _postDispatchAuditors = _registeredPostDispatchAuditors
                            .Select(x => new AuditorInstance(_auditorFactoryFunc(x.AuditorType), x.AuditRootCommandOnly)).ToArray();
                    }
                }
            }
            return _postDispatchAuditors.Where(x => isRootCommand || !x.AuditRootCommandOnly).Select(x => x.Auditor).ToArray();
        }

        private IReadOnlyCollection<ICommandAuditor> GetExecutionAuditors(bool isRootCommand)
        {
            if (_executionAuditors == null)
            {
                lock (_executionAuditorCreationLock)
                {
                    if (_executionAuditors == null)
                    {
                        _executionAuditors = _registeredExecutionAuditors
                            .Select(x => new AuditorInstance(_auditorFactoryFunc(x.AuditorType), x.AuditRootCommandOnly)).ToArray();
                    }
                }
            }
            return _executionAuditors.Where(x => isRootCommand || !x.AuditRootCommandOnly).Select(x => x.Auditor).ToArray();
        }
    }
}
