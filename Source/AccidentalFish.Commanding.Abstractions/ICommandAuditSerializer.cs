namespace AccidentalFish.Commanding.Abstractions
{
    public interface ICommandAuditSerializer
    {
        string Serialize<TCommand>(TCommand command) where TCommand : class;
    }
}
