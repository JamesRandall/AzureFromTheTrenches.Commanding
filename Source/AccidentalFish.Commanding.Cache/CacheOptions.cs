using System;
using System.Threading;

namespace AccidentalFish.Commanding.Cache
{
    public class CacheOptions
    {
        protected CacheOptions(Func<TimeSpan> lifeTimeFunc, int? maxConcurrentExecutions = null)
        {
            CommandType = null;
            MaxConcurrentExecutions = maxConcurrentExecutions;
            LifeTime = lifeTimeFunc;
            if (MaxConcurrentExecutions.HasValue)
            {
                Semaphore = new SemaphoreSlim(MaxConcurrentExecutions.Value, MaxConcurrentExecutions.Value);
            }
        }

        protected CacheOptions(Func<DateTime> expiresAtUtcFunc, int? maxConcurrentExecutions = null)
        {
            CommandType = null;
            MaxConcurrentExecutions = maxConcurrentExecutions;
            ExpiresAtUtc = expiresAtUtcFunc;
            if (MaxConcurrentExecutions.HasValue)
            {
                Semaphore = new SemaphoreSlim(MaxConcurrentExecutions.Value, MaxConcurrentExecutions.Value);
            }
        }

        private CacheOptions(Type commandType, int? maxConcurrentExecutions)
        {
            CommandType = commandType ?? throw new ArgumentNullException(nameof(commandType));
            MaxConcurrentExecutions = maxConcurrentExecutions;
            if (MaxConcurrentExecutions.HasValue)
            {
                Semaphore = new SemaphoreSlim(MaxConcurrentExecutions.Value, MaxConcurrentExecutions.Value);
            }
        }

        public CacheOptions(Type commandType, TimeSpan lifeTime, int? maxConcurrentExecutions = null) : this(commandType, maxConcurrentExecutions)
        {
            LifeTime = () => lifeTime;
        }

        public CacheOptions(Type commandType, Func<TimeSpan> lifeTimeFunc, int? maxConcurrentExecutions = null) : this(commandType, maxConcurrentExecutions)
        {
            LifeTime = lifeTimeFunc;
        }

        public CacheOptions(Type commandType, DateTime expiresAtUtc, int? maxConcurrentExecutions = null) : this(commandType, maxConcurrentExecutions)
        {
            ExpiresAtUtc = () => expiresAtUtc;
        }

        public CacheOptions(Type commandType, Func<DateTime> expiresAtUtcFunc, int? maxConcurrentExecutions = null) : this(commandType, maxConcurrentExecutions)
        {
            ExpiresAtUtc = expiresAtUtcFunc;
        }

        public Type CommandType { get; }

        public Func<TimeSpan> LifeTime { get; }

        public Func<DateTime> ExpiresAtUtc { get; }

        /// <summary>
        /// If set this throttles the number of concurrent executions that can occur of the underlying command actor
        /// when the cache misses. This can be useful when the underlying request is expensive and hitting it with multiple
        /// requests simultaneously could cause failures. This is mostly intended for scenarios with high levels of requests /
        /// transactions.
        /// Note that the implementation for this uses a double lock approach with a semaphore and so anything that finds itself
        /// blocked due to the throttle should find the result in the cache.
        /// Should further be noted that this lock is obviously within the process and not shared across servers.
        /// </summary>
        public int? MaxConcurrentExecutions { get; }

        internal SemaphoreSlim Semaphore { get; }
    }

    public class CacheOptions<T> : CacheOptions where T : class
    {
        public CacheOptions(TimeSpan lifeTime, int? maxConcurrentExecutions = null) : base(typeof(T), lifeTime, maxConcurrentExecutions)
        {

        }

        public CacheOptions(Func<TimeSpan> lifeTimeFunc, int? maxConcurrentExecutions = null) : base(typeof(T), lifeTimeFunc, maxConcurrentExecutions)
        {

        }

        public CacheOptions(DateTime expiresAtUtc, int? maxConcurrentExecutions = null) : base(typeof(T), expiresAtUtc, maxConcurrentExecutions)
        {

        }

        public CacheOptions(Func<DateTime> expiresAtUtcFunc, int? maxConcurrentExecutions = null) : base(typeof(T), expiresAtUtcFunc, maxConcurrentExecutions)
        {

        }
    }
}