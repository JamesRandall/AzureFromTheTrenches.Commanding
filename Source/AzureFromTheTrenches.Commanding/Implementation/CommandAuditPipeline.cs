using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;

namespace AzureFromTheTrenches.Commanding.Implementation
{
    internal class CommandAuditPipeline : ICommandAuditPipeline, IAuditorRegistration
    {
        private const string DispatchType = "dispatch";
        private const string ExecutionType = "execution";

        private readonly Func<Type, ICommandAuditor> _auditorFactoryFunc;
        private readonly Func<ICommandAuditSerializer> _auditorSerializerFunc;
        private readonly List<Type> _registeredDispatchAuditors = new List<Type>();
        private readonly List<Type> _registeredExecutionAuditors = new List<Type>();
        private volatile IReadOnlyCollection<ICommandAuditor> _dispatchAuditors;
        private volatile IReadOnlyCollection<ICommandAuditor> _executionAuditors;
        private readonly object _dispatchAuditorCreationLock = new object();
        private readonly object _executionAuditorCreationLock = new object();

        public CommandAuditPipeline(Func<Type, ICommandAuditor> auditorFactoryFunc, Func<ICommandAuditSerializer> auditorSerializerFunc)
        {
            _auditorFactoryFunc = auditorFactoryFunc;
            _auditorSerializerFunc = auditorSerializerFunc;
        }

        
        public void RegisterDispatchAuditor<TAuditorImpl>() where TAuditorImpl : ICommandAuditor
        {
            // all auditors must be registered before the first command is dispatched
            _registeredDispatchAuditors.Add(typeof(TAuditorImpl));
        }

        public void RegisterExecutionAuditor<TAuditorImpl>() where TAuditorImpl : ICommandAuditor
        {
            // all auditors must be registered before the first command is dispatched
            _registeredExecutionAuditors.Add(typeof(TAuditorImpl));
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
            IReadOnlyCollection<ICommandAuditor> auditors = GetDispatchAuditors();
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
            IReadOnlyCollection<ICommandAuditor> auditors = GetExecutionAuditors();
            List<Task> auditTasks = new List<Task>();
            foreach (ICommandAuditor auditor in auditors)
            {
                auditTasks.Add(auditor.Audit(auditItem));
            }
            await Task.WhenAll(auditTasks);
        }

        private IReadOnlyCollection<ICommandAuditor> GetDispatchAuditors()
        {
            if (_dispatchAuditors == null)
            {
                lock (_dispatchAuditorCreationLock)
                {
                    if (_dispatchAuditors == null)
                    {
                        _dispatchAuditors = _registeredDispatchAuditors.Select(x => _auditorFactoryFunc(x)).ToList();
                    }
                }
            }
            return _dispatchAuditors;
        }

        private IReadOnlyCollection<ICommandAuditor> GetExecutionAuditors()
        {
            if (_executionAuditors == null)
            {
                lock (_executionAuditorCreationLock)
                {
                    if (_executionAuditors == null)
                    {
                        _executionAuditors = _registeredExecutionAuditors.Select(x => _auditorFactoryFunc(x)).ToList();
                    }
                }
            }
            return _executionAuditors;
        }
    }
}
