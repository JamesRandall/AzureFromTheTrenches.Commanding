using AccidentalFish.Commanding.Abstractions;
using AccidentalFish.Commanding.Abstractions.Model;

namespace AccidentalFish.Commanding.Implementation
{
    internal class NoResultCommandWrapper : ICommand<NoResult>
    {
        public NoResultCommandWrapper(ICommand command)
        {
            Command = command;
        }

        public ICommand Command { get; }        
    }
}
