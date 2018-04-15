namespace AzureFromTheTrenches.Commanding.Abstractions
{
    /// <summary>
    /// Represents a command that can be queued.
    /// </summary>
    public interface IQueueableCommand
    {
        /// <summary>
        /// Whether or not the command should be deqeued or returned to the queue
        /// </summary>
        bool ShouldDequeue { get; set; }
        /// <summary>
        /// The number of times the command has been pulled from the queue
        /// </summary>
        int DequeueCount { get; set; }
    }
}
