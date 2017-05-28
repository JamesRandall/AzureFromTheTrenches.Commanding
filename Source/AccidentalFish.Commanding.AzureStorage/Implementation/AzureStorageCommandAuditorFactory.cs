namespace AccidentalFish.Commanding.AzureStorage.Implementation
{
    internal class AzureStorageCommandAuditorFactory : ICommandAuditorFactory
    {
        private readonly IAzureStorageCommandAuditorConfiguration _configuration;

        public AzureStorageCommandAuditorFactory(IAzureStorageCommandAuditorConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ICommandAuditor Create<TCommand>() where TCommand : class
        {
            return new AzureStorageCommandAuditor(_configuration.AuditByDateDescTable, _configuration.AuditByCorrelationIdTable, _configuration.CommandPayloadContainer);
        }
    }
}
