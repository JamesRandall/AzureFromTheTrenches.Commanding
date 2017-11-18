using AccidentalFish.Commanding.Abstractions;

namespace AccidentalFish.Commanding.Tests.Unit.TestModel
{
    internal class SimpleCommand : ICommand<SimpleResult>
    {
        public string Message { get; set; }
    }
}
