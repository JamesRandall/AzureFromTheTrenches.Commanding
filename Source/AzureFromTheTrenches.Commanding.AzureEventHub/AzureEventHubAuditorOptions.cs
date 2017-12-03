namespace AzureFromTheTrenches.Commanding.AzureEventHub
{
    public class AzureEventHubAuditorOptions
    {
        public AzureEventHubAuditorOptions()
        {
            UsePreDispatchAuditor = true;
            UseExecutionAuditor = true;
            UsePostDispatchAuditor = true;
            AuditExecuteDispatchRootOnly = true;
            AuditPostDispatchRootOnly = true;
            AuditPreDispatchRootOnly = true;
        }

        public bool UsePreDispatchAuditor { get; set; }

        public bool UsePostDispatchAuditor { get; set; }

        public bool UseExecutionAuditor { get; set; }

        public bool AuditPreDispatchRootOnly { get; set; }

        public bool AuditPostDispatchRootOnly { get; set; }

        public bool AuditExecuteDispatchRootOnly { get; set; }
    }
}
