namespace AccidentalFish.Commanding.Model
{
    public sealed class CommandChainActorResult<TResult> : ICommandResult
    {
        public CommandChainActorResult(bool shouldStop, TResult result)
        {
            ShouldStop = shouldStop;
            Result = result;
        }

        public bool ShouldStop { get; }

        public TResult Result { get; }
    }
}
