using System;
using System.Threading.Tasks;

namespace AccidentalFish.Commanding
{
    /// <summary>
    /// Interface that auditors must implement
    /// </summary>
    public interface ICommandAuditor
    {
        /// <summary>
        /// Implementations should use this to store the payload and any command index data.
        /// </summary>
        /// <typeparam name="TCommand">The command type to be audited</typeparam>
        /// <param name="command">The command</param>
        /// <param name="dispatchContext">The dispatch context</param>
        /// <returns></returns>
        Task AuditWithCommandPayload<TCommand>(TCommand command, ICommandDispatchContext dispatchContext) where TCommand : class;
        /// <summary>
        /// Implementations should use this to store command index data only
        /// </summary>
        /// <param name="commandId">The ID of the command</param>
        /// <param name="commandType">The type of the command</param>
        /// <param name="dispatchContext">The context</param>
        /// <returns></returns>
        Task AuditWithNoPayload(Guid commandId, string commandType, ICommandDispatchContext dispatchContext);
    }
}
