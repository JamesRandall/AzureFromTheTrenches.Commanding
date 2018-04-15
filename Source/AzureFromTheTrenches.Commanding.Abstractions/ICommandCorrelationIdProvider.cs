namespace AzureFromTheTrenches.Commanding.Abstractions
{
    /// <summary>
    /// Implementations of this interface will generate correlation IDs for use across the command system - used to group commands together
    /// including nested commands (a command is fired that invokes a handler that fires another command etc.)
    /// </summary>
    public interface ICommandCorrelationIdProvider
    {
        /// <summary>
        /// Create a correlation ID.
        /// Performance critical as will be called on every command dispatch.
        /// </summary>
        /// <returns>A correlation ID</returns>
        string Create();
    }
}
