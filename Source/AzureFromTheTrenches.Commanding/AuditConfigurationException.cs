using System;

namespace AzureFromTheTrenches.Commanding
{
    /// <summary>
    /// Raised if their was an error in the configuration of an auditor
    /// </summary>
    public class AuditConfigurationException : Exception
    {
        /// <inheritdoc />
        public AuditConfigurationException()
        {
        }

        /// <inheritdoc />
        public AuditConfigurationException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public AuditConfigurationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
