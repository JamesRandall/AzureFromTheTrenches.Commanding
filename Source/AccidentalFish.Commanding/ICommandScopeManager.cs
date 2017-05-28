namespace AccidentalFish.Commanding
{
    public interface ICommandScopeManager
    {
        ICommandContext Enter();
        void Exit();
    }
}
