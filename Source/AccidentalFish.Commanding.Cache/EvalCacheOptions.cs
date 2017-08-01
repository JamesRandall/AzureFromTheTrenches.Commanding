using System;

namespace AccidentalFish.Commanding.Cache
{
    public class EvalCacheOptions : CacheOptions
    {
        public Func<object,bool> IsCachedCommand { get; }

        public EvalCacheOptions(Func<object, bool> evaluator, TimeSpan lifeTime, int? maxConcurrentExecutions = null) : base(() => lifeTime, maxConcurrentExecutions)
        {
            IsCachedCommand = evaluator;
        }

        public EvalCacheOptions(Func<object, bool> evaluator, Func<TimeSpan> lifeTimeFunc, int? maxConcurrentExecutions = null) : base(lifeTimeFunc, maxConcurrentExecutions)
        {
            IsCachedCommand = evaluator;
        }

        public EvalCacheOptions(Func<object, bool> evaluator, DateTime expiresAtUtc, int? maxConcurrentExecutions = null) : base(() => expiresAtUtc, maxConcurrentExecutions)
        {
            IsCachedCommand = evaluator;
        }

        public EvalCacheOptions(Func<object, bool> evaluator, Func<DateTime> expiresAtUtcFunc, int? maxConcurrentExecutions = null) : base(expiresAtUtcFunc, maxConcurrentExecutions)
        {
            IsCachedCommand = evaluator;
        }
    }
}
