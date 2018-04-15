namespace AzureFromTheTrenches.Commanding.Abstractions
{
    /// <summary>
    /// Serializer used by the audit pipeline
    /// </summary>
    public interface ICommandAuditSerializer
    {
        /// <summary>
        /// Serialize the command
        /// </summary>
        /// <param name="command">The command to serialize</param>
        /// <returns>A serialized version of the command</returns>
        string Serialize(ICommand command);
    }
}
