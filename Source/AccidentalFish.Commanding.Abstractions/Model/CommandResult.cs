namespace AccidentalFish.Commanding.Abstractions.Model
{
    public class CommandResult
    {
        public CommandResult(bool deferExecution)
        {
            DeferExecution = deferExecution;
        }

        public bool DeferExecution { get; }
    }

    public sealed class CommandResult<TResult> : CommandResult
    {
        public CommandResult(TResult result, bool deferExecution) : base(deferExecution)
        {
            Result = result;
        }

        public TResult Result { get; }

        public static implicit operator TResult(CommandResult<TResult> commandResult)
        {
            return commandResult.Result;
        }
    }
}
