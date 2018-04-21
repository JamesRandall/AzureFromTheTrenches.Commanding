namespace AzureFromTheTrenches.Commanding.AzureFunctions.Model
{
    public abstract class AbstractFunctionDefinition
    {
        public string Namespace { get; set; }

        public string Name { get; set; }

        public string CommandType { get; set; }
    }
}
