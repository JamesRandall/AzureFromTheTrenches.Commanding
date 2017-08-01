using System;
using System.Collections.Generic;
using System.Text;

namespace AccidentalFish.Commanding.Cache
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
