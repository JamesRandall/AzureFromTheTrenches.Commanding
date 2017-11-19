using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions.Model;

namespace AzureFromTheTrenches.Commanding.Abstractions
{
    /// <summary>
    /// Interface that auditors must implement
    /// </summary>
    public interface ICommandAuditor
    {
        Task Audit(AuditItem auditItem);
    }
}
