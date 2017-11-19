using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.Tests.Unit.TestModel
{
    internal class SimpleCommand : ICommand<SimpleResult>
    {
        public string Message { get; set; }
    }
}
