using System;
using System.Collections.Generic;
using System.Text;

namespace AccidentalFish.Commanding
{
    public interface ICommandContext
    {
        string CorrelationId { get; }

        int Depth { get; }

        int Increment();

        int Decrement();
    }
}
