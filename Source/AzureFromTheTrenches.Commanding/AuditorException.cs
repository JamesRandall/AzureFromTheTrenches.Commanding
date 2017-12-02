using System;

namespace AzureFromTheTrenches.Commanding
{
    public class AuditorException : Exception
    {
        public AuditorException(string message) : base(message)
        {
        }
    }
}
