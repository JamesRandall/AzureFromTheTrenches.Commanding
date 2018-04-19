using AzureFromTheTrenches.Commanding.Abstractions;

namespace SampleFunctionConfiguration.Commands
{
    public class AddCommand : ICommand<int>
    {
        public int ValueOne { get; set; }

        public int ValueTwo { get; set; }
    }
}
