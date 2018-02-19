using System;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.Tests.Acceptance.Helpers
{
    public class SimpleCommandWithIntegerResult : ICommand<int>
    {
        public Exception OptionalException { get; }

        public SimpleCommandWithIntegerResult(Exception optionalException = null)
        {
            OptionalException = optionalException;
        }
    }
}
