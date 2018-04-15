namespace AzureFromTheTrenches.Commanding.Abstractions
{
    /// <summary>
    /// Suggested orders for command handler pipelines
    /// </summary>
    public class CommandHandlerOrder
    {
        /// <summary>
        /// Default order
        /// </summary>
        public const int Default = 1000;
        /// <summary>
        /// Pre-default handler
        /// </summary>
        public const int Pre = Default - 100;
        /// <summary>
        /// Post-default handler
        /// </summary>
        public const int Post = Default + 100;
    }
}
