namespace AzureFromTheTrenches.Commanding.Abstractions.Model
{
    public sealed class PipelineAwareCommandHandlerResult<TResult>
    {
        public PipelineAwareCommandHandlerResult(bool shouldStop, TResult result)
        {
            ShouldStop = shouldStop;
            Result = result;
        }

        public bool ShouldStop { get; }

        public TResult Result { get; }
    }
}
