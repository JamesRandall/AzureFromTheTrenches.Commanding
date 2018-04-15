using System.Threading;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions.Model;

namespace AzureFromTheTrenches.Commanding.Abstractions
{
    /// <summary>
    /// Interface that auditors must implement
    /// </summary>
    public interface ICommandAuditor
    {
        /// <summary>
        /// Should perform an audit by sending the AuditItem to the store
        /// </summary>
        /// <param name="auditItem">The item to audit</param>
        /// <param name="cancellationToken">A cancellation token</param>
        /// <returns>Awaitable task</returns>
        Task Audit(AuditItem auditItem, CancellationToken cancellationToken);
    }
}
