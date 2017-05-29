namespace AccidentalFish.Commanding.Implementation
{
    internal interface ICommandDispatcherOptions
    {
        bool? AuditRootCommandOnly { get; }
    }
}
