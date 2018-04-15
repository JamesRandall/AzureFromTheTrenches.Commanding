namespace AzureFromTheTrenches.Commanding.Abstractions
{
    /// <summary>
    /// Represents a command
    /// </summary>
    public interface ICommand
    {
        
    }

    /// <summary>
    /// Represents a command that returns a result
    /// </summary>
    /// <typeparam name="TResult">The type of the result</typeparam>
    public interface ICommand<TResult> : ICommand
    {
    }
}
