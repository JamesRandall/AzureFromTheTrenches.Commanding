using System;
using System.Collections.Generic;
using System.Text;

namespace AzureFromTheTrenches.Commanding.Abstractions
{
    #region Base interfaces for type safety in implementation and generic support
    public interface ICommandHandlerBase
    {

    }

    public interface ICommandHandler : ICommandHandlerBase
    {

    }

    public interface ICancellableCommandHandler : ICommandHandler
    {

    }

    public interface IPipelineAwareCommandHandler : ICommandHandlerBase
    {

    }

    public interface ICancellablePipelineAwareCommandHandler : IPipelineAwareCommandHandler
    {

    }
    #endregion
}
