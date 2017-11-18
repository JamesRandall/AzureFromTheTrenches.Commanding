using System;
using System.Threading.Tasks;
using AccidentalFish.Commanding.Model;

namespace AccidentalFish.Commanding
{
    /// <summary>
    /// Interface that auditors must implement
    /// </summary>
    public interface ICommandAuditor
    {
        Task Audit(AuditItem auditItem);
    }
}
