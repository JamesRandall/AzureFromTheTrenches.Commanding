using AccidentalFish.Commanding.Abstractions;

namespace AccidentalFish.Commanding.Cache.Tests.Unit.TestModel
{
    internal class SimpleCommand : ICommand<SimpleResult>
    {
        public int SomeValue { get; set; }

        public int AnotherValue { get; set; }
    }
}
