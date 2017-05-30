using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccidentalFish.Commanding.Implementation
{
    internal class CommandAuditPipeline : ICommandAuditPipeline, ICommandAuditor
    {
        private readonly Func<Type, ICommandAuditor> _auditorFactoryFunc;
        private readonly List<Type> _registeredAuditors = new List<Type>();
        private volatile IReadOnlyCollection<ICommandAuditor> _auditors = null;
        private readonly object _auditorCreationLock = new object();

        public CommandAuditPipeline(Func<Type, ICommandAuditor> auditorFactoryFunc)
        {
            _auditorFactoryFunc = auditorFactoryFunc;
        }

        
        public void RegisterAuditor<TAuditorImpl>() where TAuditorImpl : ICommandAuditor
        {
            // all auditors must be registered before the first command is dispatched
            _registeredAuditors.Add(typeof(TAuditorImpl));
        }

        public async Task Audit<TCommand>(TCommand command, ICommandDispatchContext dispatchContext) where TCommand : class
        {
            IReadOnlyCollection<ICommandAuditor> auditors = GetAuditors();
            List<Task> auditTasks = new List<Task>();
            foreach (ICommandAuditor auditor in auditors)
            {
                auditTasks.Add(auditor.Audit(command, dispatchContext));
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
