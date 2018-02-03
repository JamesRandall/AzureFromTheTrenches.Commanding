namespace AzureFromTheTrenches.Commanding.Implementation
{
    class OptionsProvider : IOptionsProvider
    {
        public OptionsProvider(Options options)
        {
            Options = options;
        }

        public Options Options { get; }
    }
}
