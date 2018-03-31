using System.Threading.Tasks;
using AspNetCoreConfigurationBasedCommandControllers.Commands;
using AspNetCoreConfigurationBasedCommandControllers.Commands.Responses;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AspNetCoreConfigurationBasedCommandControllers.Handlers
{
    public class GetPropertyValueQueryHandler : ICommandHandler<GetPropertyValueQuery, PropertyValue>
    {
        public Task<PropertyValue> ExecuteAsync(GetPropertyValueQuery command, PropertyValue previousResult)
        {
            if (SimpleRepository.Values.TryGetValue(command.Fqn, out PropertyValue value))
            {
                return Task.FromResult(value);
            }

            return Task.FromResult<PropertyValue>(null);
        }
    }
}
