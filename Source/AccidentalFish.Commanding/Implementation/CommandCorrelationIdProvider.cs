using System;

namespace AccidentalFish.Commanding.Implementation
{
    class CommandCorrelationIdProvider : ICommandCorrelationIdProvider
    {
        public string Create()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
