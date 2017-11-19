using System;

namespace AzureFromTheTrenches.Commanding.Cache
{
    class CacheKeyException : Exception
    {
        public CacheKeyException()
        {
        }

        public CacheKeyException(string message) : base(message)
        {
        }

        public CacheKeyException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
