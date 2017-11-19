namespace AzureFromTheTrenches.Commanding.Cache
{
    public interface ICacheKeyProvider
    {
        string CacheKey<T>(T command);
    }
}
