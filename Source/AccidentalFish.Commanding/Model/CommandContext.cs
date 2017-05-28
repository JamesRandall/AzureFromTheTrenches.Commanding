using System.Threading;

namespace AccidentalFish.Commanding.Model
{
    internal class CommandContext : ICommandContext
    {
        private int _depth;

        public CommandContext(string correlationId)
        {
            CorrelationId = correlationId;
            _depth = 0;
        }

        public string CorrelationId { get; }

        public int Depth => _depth;

        public int Increment()
        {
            return Interlocked.Increment(ref _depth);
        }

        public int Decrement()
        {
            return Interlocked.Decrement(ref _depth);
        }
    }
}
