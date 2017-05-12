using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AccidentalFish.Commanding.Queue.Implementation
{
    internal class AsynchronousBackoffPolicy : IAsynchronousBackoffPolicy
    {
        private Action _shutdownAction;
        private Action<string> _logAction;
        private int _backoffIndex = -1;
        private CancellationToken _cancellationToken;
        private TimeSpan[] _backoffTimings;

        private static readonly IReadOnlyCollection<TimeSpan> DefaultBackoffTimings = new ReadOnlyCollection<TimeSpan>(new[]
        {
            TimeSpan.FromMilliseconds(100),
            TimeSpan.FromMilliseconds(250),
            TimeSpan.FromMilliseconds(500),
            TimeSpan.FromMilliseconds(1000),
            TimeSpan.FromMilliseconds(5000)
        });
        
        public async Task ExecuteAsync(Func<Task<bool>> function, CancellationToken cancellationToken)
        {
            await ExecuteAsync(function, null, null, null, cancellationToken);
        }

        public async Task ExecuteAsync(Func<Task<bool>> function, Action shutdownAction, Action<string> logAction, IEnumerable<TimeSpan> backoffTimings, CancellationToken cancellationToken)
        {
            _shutdownAction = shutdownAction;
            _logAction = logAction;
            _cancellationToken = cancellationToken;
            _backoffTimings = backoffTimings?.ToArray() ?? DefaultBackoffTimings.ToArray();

            bool shouldContinue = true;

            do
            {
                bool didWork = await function();
                if (!didWork)
                {
                    shouldContinue = await Backoff();
                }
                else
                {
                    _backoffIndex = -1;
                }

                if (_cancellationToken.IsCancellationRequested)
                {
                    shouldContinue = false;
                }
            } while (shouldContinue);
            _shutdownAction?.Invoke();
        }

        private async Task<bool> Backoff()
        {

            _backoffIndex++;
            if (_backoffIndex >= _backoffTimings.Length)
            {
                _backoffIndex = _backoffTimings.Length - 1;
            }
            try
            {
                _logAction?.Invoke($"AsynchronousBackoffPolicy - backing off for {_backoffTimings[_backoffIndex].TotalMilliseconds}ms");

                await Task.Delay(_backoffTimings[_backoffIndex], _cancellationToken);
                return true;
            }
            catch (TaskCanceledException)
            {
                return false;
            }
        }
    }
}
