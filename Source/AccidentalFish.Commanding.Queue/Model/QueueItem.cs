using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureFromTheTrenches.Commanding.Queue.Model
{
    public class QueueItem<T> where T : class
    {
        public QueueItem(T item, int dequeueCount, string popReceipt, IReadOnlyDictionary<string, object> properties, Func<Task> extendLeaseAsync)
        {
            Item = item;
            DequeueCount = dequeueCount;
            PopReciept = popReceipt;
            Properties = properties;
            ExtendLeaseAsync = extendLeaseAsync;
        }

        public T Item { get; }
        public int DequeueCount { get; }
        public string PopReciept { get; }
        public IReadOnlyDictionary<string, object> Properties { get; }
        public Func<Task> ExtendLeaseAsync { get; }
    }
}
