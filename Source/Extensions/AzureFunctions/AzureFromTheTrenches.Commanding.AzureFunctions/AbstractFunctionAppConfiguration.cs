namespace AzureFromTheTrenches.Commanding.AzureFunctions
{
    public interface IFunctionAppConfiguration
    {
        void Build(IFunctionHostBuilder builder);
    }
}
