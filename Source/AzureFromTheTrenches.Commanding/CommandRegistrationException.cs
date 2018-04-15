using System;

namespace AzureFromTheTrenches.Commanding
{
    /// <summary>
    /// An exception thrown if their is an issue registering commands
    /// </summary>
    public class CommandRegistrationException : Exception
    {
        /// <summary>
        /// Conmstructor
        /// </summary>
        /// <param name="message">The error message</param>
        public CommandRegistrationException(string message) : base(message)
        {
        }
    }
}
