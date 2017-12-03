namespace AzureFromTheTrenches.Commanding.MicrosoftLoggingExtensions
{
    public class MicrosoftLoggingExtensionsAuditorOptions
    {
        public MicrosoftLoggingExtensionsAuditorOptions()
        {
            UsePreDispatchAuditor = true;
            UseExecutionAuditor = true;
            UsePostDispatchAuditor = true;
            AuditExecuteDispatchRootOnly = false;
            AuditPostDispatchRootOnly = false;
            AuditPreDispatchRootOnly = false;
        }

        public bool UsePreDispatchAuditor { get; set; }

        public bool UsePostDispatchAuditor { get; set; }

        public bool UseExecutionAuditor { get; set; }

        public bool AuditPreDispatchRootOnly { get; set; }

        public bool AuditPostDispatchRootOnly { get; set; }

        public bool AuditExecuteDispatchRootOnly { get; set; }
    }
}
