using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.Http.Tests.Unit.TestInfrastructure
{
    internal class SimpleCommand : ICommand
    {
        public int SomeNumber { get; set; }

        public string Message { get; set; }
    }
}
