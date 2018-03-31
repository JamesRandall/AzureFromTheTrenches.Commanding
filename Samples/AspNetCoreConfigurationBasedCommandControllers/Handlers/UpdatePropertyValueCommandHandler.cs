using System.Threading.Tasks;
using AspNetCoreConfigurationBasedCommandControllers.Commands;
using AspNetCoreConfigurationBasedCommandControllers.Commands.Responses;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AspNetCoreConfigurationBasedCommandControllers.Handlers
{
    public class UpdatePropertyValueCommandHandler : ICommandHandler<UpdatePropertyValueCommand>
    {
        public Task ExecuteAsync(UpdatePropertyValueCommand command)
        {
            SimpleRepository.Values.AddOrUpdate(command.PropertyFqn,
                key => new PropertyValue {PropertyFqn = command.PropertyFqn, Value = command.Value},
                (key,existing) => new PropertyValue{ PropertyFqn = existing.PropertyFqn, Value = command.Value});
            return Task.CompletedTask;
        }
    }
}
