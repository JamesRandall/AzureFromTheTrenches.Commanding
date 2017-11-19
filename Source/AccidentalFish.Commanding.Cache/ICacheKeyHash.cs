namespace AzureFromTheTrenches.Commanding.Cache
{
    /// <summary>
    /// Implementations of this interface generate a deterministic hash from the given key.
    /// The built-in GetHashcode is not appropriate to use, particularly in distributed scenarios,
    /// as it is only deterministic within a single execution / process space in .Net Core and
    /// in full framework although more reliably it is still liable to change.
    /// </summary>
    public interface ICacheKeyHash
    {
        string GetHash(string key);
    }
}
