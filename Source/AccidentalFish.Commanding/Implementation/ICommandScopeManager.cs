namespace AccidentalFish.Commanding.Implementation
{
    internal interface ICommandScopeManager
    {
        ICommandContext Enter();
        void Exit();
        ICommandContext GetCurrent();
    }
}
