using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.Implementation
{
    class OptionsProvider : IOptionsProvider
    {
        public OptionsProvider(IOptions options)
        {
            Options = options;
        }

        public IOptions Options { get; }
    }
}
