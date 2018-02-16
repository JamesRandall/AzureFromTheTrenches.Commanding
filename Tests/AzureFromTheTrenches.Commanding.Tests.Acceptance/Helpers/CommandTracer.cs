using System.Collections.Generic;

namespace AzureFromTheTrenches.Commanding.Tests.Acceptance.Helpers
{
    class CommandTracer : ICommandTracer
    {
        private readonly List<string> _loggedItems = new List<string>();

        public void Log(string text)
        {
            _loggedItems.Add(text);
        }

        public IReadOnlyCollection<string> LoggedItems => _loggedItems;
    }
}
