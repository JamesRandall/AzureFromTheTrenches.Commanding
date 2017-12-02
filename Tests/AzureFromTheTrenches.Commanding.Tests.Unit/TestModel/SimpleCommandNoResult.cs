using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.Tests.Unit.TestModel
{
    class SimpleCommandNoResult : ICommand
    {
        public bool WasHandled { get; set; }
    }
}
