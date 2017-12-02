namespace AzureFromTheTrenches.Commanding.Abstractions
{
    /// <inheritdoc />
    /// <summary>
    /// The direct command executer should be used in scenarios where dispatch has not occurred in the same
    /// process (for example during remote execution)
    /// </summary>
    public interface IDirectCommandExecuter : ICommandExecuter
    {
    }
}
