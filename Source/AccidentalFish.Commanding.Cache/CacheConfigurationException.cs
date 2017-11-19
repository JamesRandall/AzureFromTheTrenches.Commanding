using System;

namespace AzureFromTheTrenches.Commanding.Cache
{
    public class CacheConfigurationException : Exception
    {
        public CacheConfigurationException()
        {
        }

        public CacheConfigurationException(string message) : base(message)
        {
        }

        public CacheConfigurationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
