using System.Collections.Concurrent;
using AspNetCoreConfigurationBasedCommandControllers.Commands.Responses;

namespace AspNetCoreConfigurationBasedCommandControllers.Handlers
{
    public class SimpleRepository
    {
        public static readonly ConcurrentDictionary<string, PropertyValue> Values = new ConcurrentDictionary<string, PropertyValue>();
    }
}
