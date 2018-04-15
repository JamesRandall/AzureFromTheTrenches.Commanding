using System;

namespace AzureFromTheTrenches.Commanding.Abstractions
{
    /// <summary>
    /// Exception raised when the commanding system configuraiton is invalid
    /// </summary>
    public class CommandFrameworkConfigurationException : Exception
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Error message</param>
        public CommandFrameworkConfigurationException(string message) : base(message)
        {
        }
    }
}
