namespace AccidentalFish.Commanding
{
    public interface IQueueableCommand
    {
        bool ShouldDequeue { get; set; }

        int DequeueCount { get; set; }
    }
}
