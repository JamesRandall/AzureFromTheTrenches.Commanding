using System.Collections.Generic;

namespace AzureFromTheTrenches.Commanding.Tests.Acceptance.Helpers
{
    public interface ICommandTracer
    {
        void Log(string text);
        IReadOnlyCollection<string> LoggedItems { get; }
    }
}
