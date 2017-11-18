using System.Threading.Tasks;
using AccidentalFish.Commanding.Abstractions.Model;

namespace AccidentalFish.Commanding.Abstractions
{
    /// <summary>
    /// Interface that auditors must implement
    /// </summary>
    public interface ICommandAuditor
    {
        Task Audit(AuditItem auditItem);
    }
}
