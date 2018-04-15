namespace AzureFromTheTrenches.Commanding.Abstractions
{
    /// <summary>
    /// An interface that can be implemented on a command to provide a unique ID the auditor can use.
    /// This can be useful if you need to correlate the audit with external events that naturally form part of your command model.
    /// </summary>
    public interface IIdentifiableCommand
    {
        /// <summary>
        /// Returns the command ID.
        /// </summary>
        string CommandId { get; }
    }
}
