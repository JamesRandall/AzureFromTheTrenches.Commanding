namespace AzureFromTheTrenches.Commanding.Implementation
{
    internal interface ICommandDispatcherOptions
    {
        bool? AuditRootCommandOnly { get; }
    }
}
