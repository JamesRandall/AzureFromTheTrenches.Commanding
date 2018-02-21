using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;

namespace HttpCommanding.Web.Auditors
{
    public class ExecutionAuditor : ICommandAuditor
    {
        public Task Audit(AuditItem auditItem, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }
    }
}
