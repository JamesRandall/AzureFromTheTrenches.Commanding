using System;

namespace AccidentalFish.Commanding.Cache.Implementation
{
    interface IPropertyCacheKeyProviderCompiler
    {
        Func<TCommand, string> Compile<TCommand>();
    }
}
