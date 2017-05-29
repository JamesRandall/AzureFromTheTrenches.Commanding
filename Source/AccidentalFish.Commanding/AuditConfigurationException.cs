using System;

namespace AccidentalFish.Commanding
{
    public class AuditConfigurationException : Exception
    {
        public AuditConfigurationException()
        {
        }

        public AuditConfigurationException(string message) : base(message)
        {
        }

        public AuditConfigurationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
