using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using SampleFunctionConfiguration.Commands;
using SampleFunctionConfiguration.Services;

namespace SampleFunctionConfiguration.Handlers
{
    class AddCommandHandler : ICommandHandler<AddCommand, int>
    {
        private readonly ICalculator _calculator;

        public AddCommandHandler(ICalculator calculator)
        {
            _calculator = calculator;
        }

        public Task<int> ExecuteAsync(AddCommand command, int previousResult)
        {
            return Task.FromResult(_calculator.Add(command.ValueOne, command.ValueTwo));
        }
    }
}
