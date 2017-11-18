using AccidentalFish.Commanding.Abstractions;

namespace AccidentalFish.Commanding.Implementation
{
    class NullCommandAuditorFactory : ICommandAuditorFactory
    {
        public ICommandAuditor Create<TCommand>() where TCommand : class
        {
            return null;
        }
    }
}
