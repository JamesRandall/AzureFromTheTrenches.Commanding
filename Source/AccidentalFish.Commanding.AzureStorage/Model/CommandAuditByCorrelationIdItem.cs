namespace AzureFromTheTrenches.Commanding.AzureStorage.Model
{
    /// <summary>
    /// This class only extends the base class with type information to allow us to
    /// apply strategies with compile time type information as opposed to runtime.
    /// The latter being much slower.
    /// </summary>
    public class CommandAuditByCorrelationIdItem : AbstractCommandAuditTableEntity
    {
        
    }
}
