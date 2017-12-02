using System;

namespace AzureFromTheTrenches.Commanding.AzureStorage
{
    public class AzureStorageConfigurationException : Exception
    {
        public AzureStorageConfigurationException(string message) : base(message)
        {
        }
    }
}
