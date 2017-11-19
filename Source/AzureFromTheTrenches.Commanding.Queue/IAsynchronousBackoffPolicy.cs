using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AzureFromTheTrenches.Commanding.Queue
{
    /// <summary>
    /// Asynchronous backoff policy that is used by deferred command processors
    /// </summary>
    public interface IAsynchronousBackoffPolicy
    {
        /// <summary>
        /// Execute an action inside the backoff policy
        /// </summary>
        /// <param name="function">Function to execute - should return true if it the function had work to perform, false if it did not and the back off policy
        /// needs to begin</param>
        /// <param name="token">A cancellation token</param>
        /// <returns>An awaitable task</returns>
        Task ExecuteAsync(Func<Task<bool>> function, CancellationToken token);

        /// <summary>
        /// Execute an action inside the backoff policy
        /// </summary>
        /// <param name="function">Function to execute - should return true if it the function had work to perform, false if it did not and the back off policy
        /// needs to begin</param>
        /// <param name="shutdownAction">Action that is called as the policy shuts down - if null no shutdown action is called</param>
        /// <param name="logAction">Action that is called to log diagnostics - if null no logging occurs</param>
        /// <param name="backoffTimings">Optional set of back off timings. If null defaults to 100, 250, 500, 1000, 5000ms.</param>
        /// <param name="cancellationToken">A cancellation token</param>
        /// <returns>An awaitable task</returns>
        Task ExecuteAsync(Func<Task<bool>> function, Action shutdownAction, Action<string> logAction, IEnumerable<TimeSpan> backoffTimings, CancellationToken cancellationToken);
    }
}
