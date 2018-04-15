namespace AzureFromTheTrenches.Commanding.Abstractions
{
    #region Base interfaces for type safety in implementation and generic support
    /// <summary>
    /// Base interface for all command handlers
    /// </summary>
    public interface ICommandHandlerBase
    {

    }

    /// <summary>
    /// A basic command handler
    /// </summary>
    public interface ICommandHandler : ICommandHandlerBase
    {

    }

    /// <summary>
    /// A command handler that takes a cancellation token
    /// </summary>
    public interface ICancellableCommandHandler : ICommandHandler
    {

    }

    /// <summary>
    /// A pipeline aware command handler that can cancel pipeline execution
    /// </summary>
    public interface IPipelineAwareCommandHandler : ICommandHandlerBase
    {

    }

    /// <summary>
    /// A pipeline aware command handler that can cancel pipeline execution and takes a cancellation token
    /// </summary>
    public interface ICancellablePipelineAwareCommandHandler : IPipelineAwareCommandHandler
    {

    }
    #endregion
}
