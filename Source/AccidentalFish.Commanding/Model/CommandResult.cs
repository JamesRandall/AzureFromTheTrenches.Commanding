using System;

namespace AccidentalFish.Commanding.Model
{
    public sealed class CommandResult<TResult> : ICommandResult
    {
        public CommandResult(TResult result, bool deferExecution)
        {
            Result = result;
            DeferExecution = deferExecution;
        }

        public TResult Result { get; }

        public bool DeferExecution { get; }

        public static implicit operator TResult(CommandResult<TResult> commandResult)
        {
            return commandResult.Result;
        }
    }
}
