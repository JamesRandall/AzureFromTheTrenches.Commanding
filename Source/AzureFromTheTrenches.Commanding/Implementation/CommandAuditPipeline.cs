using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;
using AzureFromTheTrenches.Commanding.Model;

namespace AzureFromTheTrenches.Commanding.Implementation
{
    internal class CommandAuditPipeline : ICommandAuditPipeline, IAuditorRegistration
    {
        private const string DispatchType = "dispatch";
        private const string ExecutionType = "execution";

        private readonly Func<Type, ICommandAuditor> _auditorFactoryFunc;
        private readonly Func<ICommandAuditSerializer> _auditorSerializerFunc;
        private readonly List<AuditorDefinition> _registeredDispatchAuditors = new List<AuditorDefinition>();
        private readonly List<AuditorDefinition> _registeredExecutionAuditors = new List<AuditorDefinition>();
        private volatile IReadOnlyCollection<AuditorInstance> _dispatchAuditors;
        private volatile IReadOnlyCollection<AuditorInstance> _executionAuditors;
        private readonly object _dispatchAuditorCreationLock = new object();
        private readonly object _executionAuditorCreationLock = new object();

        public CommandAuditPipeline(Func<Type, ICommandAuditor> auditorFactoryFunc, Func<ICommandAuditSerializer> auditorSerializerFunc)
        {
            _auditorFactoryFunc = auditorFactoryFunc;
            _auditorSerializerFunc = auditorSerializerFunc;
        }

        
        public void RegisterDispatchAuditor<TAuditorImpl>(bool auditRootCommandOnly) where TAuditorImpl : ICommandAuditor
        {
            // all auditors must be registered before the first command is dispatched
            _registeredDispatchAuditors.Add(new AuditorDefinition(typeof(TAuditorImpl), auditRootCommandOnly));
        }

        public void RegisterExecutionAuditor<TAuditorImpl>(bool auditRootCommandOnly) where TAuditorImpl : ICommandAuditor
        {
            // all auditors must be registered before the first command is dispatched
            _registeredExecutionAuditors.Add(new AuditorDefinition(typeof(TAuditorImpl), auditRootCommandOnly));
        }

        public async Task AuditDispatch(ICommand command, ICommandDispatchContext dispatchContext)
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
                Type = DispatchType
            };
            // ReSharper disable once SuspiciousTypeConversion.Global - used by consumers of the package
            if (command is IIdentifiableCommand identifiableCommand)
            {
                auditItem.CommandId = identifiableCommand.Id;
            }
            await AuditDispatch(auditItem);
        }

        public async Task AuditDispatch(AuditItem auditItem)
        {
            auditItem.Type = DispatchType;
            IReadOnlyCollection<ICommandAuditor> auditors = GetDispatchAuditors(auditItem.Depth == 0);
            List<Task> auditTasks = new List<Task>();
            foreach (ICommandAuditor auditor in auditors)
            {
                auditTasks.Add(auditor.Audit(auditItem));
            }
            await Task.WhenAll(auditTasks);
        }

        public async Task AuditExecution(ICommand command, ICommandDispatchContext dispatchContext,
            bool executedSuccessfully)
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
            await AuditExecution(auditItem);
        }

        public async Task AuditExecution(AuditItem auditItem)
        {
            auditItem.Type = ExecutionType;
            IReadOnlyCollection<ICommandAuditor> auditors = GetExecutionAuditors(auditItem.Depth == 0);
            List<Task> auditTasks = new List<Task>();
            foreach (ICommandAuditor auditor in auditors)
            {
                auditTasks.Add(auditor.Audit(auditItem));
            }
            await Task.WhenAll(auditTasks);
        }

        private IReadOnlyCollection<ICommandAuditor> GetDispatchAuditors(bool isRootCommand)
        {
            if (_dispatchAuditors == null)
            {
                lock (_dispatchAuditorCreationLock)
                {
                    if (_dispatchAuditors == null)
                    {
                        _dispatchAuditors = _registeredDispatchAuditors
                            .Select(x => new AuditorInstance(_auditorFactoryFunc(x.AuditorType), x.AuditRootCommandOnly)).ToArray();
                    }
                }
            }
            return _dispatchAuditors.Where(x => isRootCommand || !x.AuditRootCommandOnly).Select(x => x.Auditor).ToArray();
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
