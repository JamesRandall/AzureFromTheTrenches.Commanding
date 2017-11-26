using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AzureFromTheTrenches.Commanding.Abstractions
{
    /// <summary>
    /// This interface can be implemented to catch and handle errors that occur in execution
    /// </summary>
    public interface ICommandExecutionExceptionHandler
    {
        /// <summary>
        /// Handles an exception in command execution, if the handler does not rethrow the exception
        /// it can return true to continue to run registered handlers or false to stop running registered handlers
        /// </summary>
        /// <typeparam name="TResult">Type of the command result</typeparam>
        /// <param name="ex">Exception raised</param>
        /// <param name="handler">The handler that caused the exception</param>
        /// <param name="handlerExecutionIndex">The index in theexecution sequence of the handler</param>
        /// <param name="command">The command being executed</param>
        /// <param name="dispatchContext">The current dispatch status</param>
        /// <returns>True to continue, false to stop</returns>
        Task<bool> HandleException<TResult>(Exception ex, object handler, int handlerExecutionIndex, ICommand<TResult> command, ICommandDispatchContext dispatchContext);
    }
}
