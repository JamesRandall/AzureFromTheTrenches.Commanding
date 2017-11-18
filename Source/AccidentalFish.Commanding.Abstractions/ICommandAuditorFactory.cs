namespace AccidentalFish.Commanding.Abstractions
{
    public interface ICommandAuditorFactory
    {
        ICommandAuditor Create<TCommand>() where TCommand : class;
    }
}
