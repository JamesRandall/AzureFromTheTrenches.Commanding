using System;

namespace AccidentalFish.Commanding.Cache
{
    public interface ICacheKeyProvider
    {
        string CacheKey<T>(T command);
    }
}
