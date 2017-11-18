namespace AccidentalFish.Commanding.Abstractions
{
    public interface ICommand
    {
        
    }

    public interface ICommand<TResult> : ICommand
    {
    }
}
