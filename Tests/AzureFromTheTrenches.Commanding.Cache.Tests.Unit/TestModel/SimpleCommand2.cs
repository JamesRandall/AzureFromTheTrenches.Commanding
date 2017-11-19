using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.Cache.Tests.Unit.TestModel
{
    public class SimpleCommand2 : ICommand<SimpleResult>
    {
        public int SomeValue { get; set; }

        public int AnotherValue { get; set; }
    }
}
