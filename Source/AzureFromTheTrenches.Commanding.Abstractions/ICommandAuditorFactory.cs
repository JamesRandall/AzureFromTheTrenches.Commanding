namespace AzureFromTheTrenches.Commanding.Abstractions
{
    /// <summary>
    /// Implementations of this interface create auditors for given types of command
    /// </summary>
    public interface ICommandAuditorFactory
    {
        /// <summary>
        /// Create an auditor
        /// </summary>
        /// <typeparam name="TCommand">The command type associated with the auditor</typeparam>
        /// <returns>The auditor</returns>
        ICommandAuditor Create<TCommand>() where TCommand : class;
    }
}
