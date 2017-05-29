namespace AccidentalFish.Commanding.Implementation
{
    class CommandDispatcherOptions : ICommandDispatcherOptions
    {
        public CommandDispatcherOptions(bool? auditRootCommandOnly)
        {
            AuditRootCommandOnly = auditRootCommandOnly;
        }

        public bool? AuditRootCommandOnly { get; }
    }
}
