namespace AccidentalFish.Commanding.Abstractions.Model
{
    public sealed class CommandChainActorResult<TResult>
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
