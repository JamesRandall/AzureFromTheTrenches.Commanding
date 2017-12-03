using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.Tests.Unit.TestModel
{
    class SimpleIdentifiableCommand : ICommand, IIdentifiableCommand
    {
        public string CommandId { get; set; }
    }
}
