using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccidentalFish.Commanding.Model;

namespace AccidentalFish.Commanding.Implementation
{
    internal class CommandAuditPipeline : ICommandAuditPipeline, IAuditorRegistration
    {
        private readonly Func<Type, ICommandAuditor> _auditorFactoryFunc;
        private readonly Func<ICommandAuditSerializer> _auditorSerializerFunc;
        private readonly List<Type> _registeredAuditors = new List<Type>();
        private volatile IReadOnlyCollection<ICommandAuditor> _auditors = null;
        private readonly object _auditorCreationLock = new object();

        public CommandAuditPipeline(Func<Type, ICommandAuditor> auditorFactoryFunc, Func<ICommandAuditSerializer> auditorSerializerFunc)
        {
            _auditorFactoryFunc = auditorFactoryFunc;
            _auditorSerializerFunc = auditorSerializerFunc;
        }

        
        public void RegisterAuditor<TAuditorImpl>() where TAuditorImpl : ICommandAuditor
        {
            // all auditors must be registered before the first command is dispatched
            _registeredAuditors.Add(typeof(TAuditorImpl));
        }

        public async Task Audit<TCommand>(TCommand command, Guid commandId, ICommandDispatchContext dispatchContext) where TCommand : class
        {
            ICommandAuditSerializer serializer = _auditorSerializerFunc();
            AuditItem auditItem = new AuditItem
            {
                AdditionalProperties = dispatchContext.AdditionalProperties.ToDictionary(x => x.Key, x => x.Value.ToString()),
                CommandId = commandId,
                CommandType = command.GetType().AssemblyQualifiedName,
                CorrelationId = dispatchContext.CorrelationId,
                Depth = dispatchContext.Depth,
                DispatchedUtc = DateTime.UtcNow,
                SerializedCommand = serializer.Serialize(command)
            };
            await Audit(auditItem);
        }

        public async Task Audit(AuditItem auditItem)
        {
            IReadOnlyCollection<ICommandAuditor> auditors = GetAuditors();
            List<Task> auditTasks = new List<Task>();
            foreach (ICommandAuditor auditor in auditors)
            {
                auditTasks.Add(auditor.Audit(auditItem));
            }
            await Task.WhenAll(auditTasks);
        }

        private IReadOnlyCollection<ICommandAuditor> GetAuditors()
        {
            if (_auditors == null)
            {
                lock (_auditorCreationLock)
                {
                    if (_auditors == null)
                    {
                        _auditors = _registeredAuditors.Select(x => _auditorFactoryFunc(x)).ToList();
                    }
                }
            }
            return _auditors;
        }
    }
}
