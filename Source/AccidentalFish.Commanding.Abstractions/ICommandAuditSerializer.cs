namespace AccidentalFish.Commanding.Abstractions
{
    public interface ICommandAuditSerializer
    {
        string Serialize(ICommand command);
    }
}
