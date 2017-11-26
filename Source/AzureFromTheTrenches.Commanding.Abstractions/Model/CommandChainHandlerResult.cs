namespace AzureFromTheTrenches.Commanding.Abstractions.Model
{
    public sealed class CommandChainHandlerResult<TResult>
    {
        public CommandChainHandlerResult(bool shouldStop, TResult result)
        {
            ShouldStop = shouldStop;
            Result = result;
        }

        public bool ShouldStop { get; }

        public TResult Result { get; }
    }
}
