namespace AccidentalFish.Commanding
{
    public interface ICommandAuditorFactory
    {
        ICommandAuditor Create<TCommand>() where TCommand : class;
    }
}
