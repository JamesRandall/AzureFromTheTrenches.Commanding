using System.Threading;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.Implementation
{
    class LocallyUniqueCommandCorrelationIdProvider : ICommandCorrelationIdProvider
    {
        private static long _nextId;

        public string Create()
        {
            long id = Interlocked.Increment(ref _nextId);
            return id.ToString();
        }
    }
}
